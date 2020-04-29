using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(MapEvent), "SimulateSingleHit")]
    public class SimulateSingleHitPatch
    {
        private static MethodInfo MapEventSide_ApplySimulationDamageToSelectedTroop = typeof(MapEventSide).GetMethod(
            "ApplySimulationDamageToSelectedTroop", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static FieldInfo MapEventSide__selectedSimulationTroopIndex = typeof(MapEventSide).GetField(
            "_selectedSimulationTroopIndex", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static FieldInfo MapEventSide__selectedSimulationTroopDescriptor = typeof(MapEventSide).GetField(
            "_selectedSimulationTroopDescriptor", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static FieldInfo MapEventSide__simulationTroopList = typeof(MapEventSide).GetField(
            "_simulationTroopList", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static FieldInfo MapEventSide__selectedSimulationTroop = typeof(MapEventSide).GetField(
            "_selectedSimulationTroop", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static UniqueTroopDescriptor SelectSimulationTroopAtIndex(MapEventSide side, int index)
        {
            // side._selectedSimulationTroopIndex = index;
            MapEventSide__selectedSimulationTroopIndex.SetValue(side, index);

            // side._selectedSimulationTroopDescriptor = side._simulationTroopList[index];
            UniqueTroopDescriptor selectedSimulationTroopDescriptor = ((List<UniqueTroopDescriptor>)MapEventSide__simulationTroopList.GetValue(side))[index];

            MapEventSide__selectedSimulationTroopDescriptor.SetValue(side, selectedSimulationTroopDescriptor);

            // side._selectedSimulationTroop = side.GetAllocatedTroop(side._selectedSimulationTroopDescriptor);
            MapEventSide__selectedSimulationTroop.SetValue(side, side.GetAllocatedTroop(selectedSimulationTroopDescriptor));

            return selectedSimulationTroopDescriptor;
        }

        private static bool StrikeOnce(MapEvent __instance,
            MapEventSide strikerSide,
            MapEventSide strikedSide,
            float distributedOffenseRating,
            out float totalDamageDone)
        {
            int strikerNumber = strikerSide.NumRemainingSimulationTroops;
            int strikedNumber = strikedSide.NumRemainingSimulationTroops;
            totalDamageDone = 0;
            if (strikerNumber == 0 || strikedNumber == 0)
            {
                return true;
            }

            bool finishedAnyone = false;          
            for (int index = strikedNumber - 1; index >= 0; index--)
            {
                UniqueTroopDescriptor strikerTroopDescriptor = strikerSide.SelectRandomSimulationTroop();
                CharacterObject strikerTroop = strikerSide.GetAllocatedTroop(strikerTroopDescriptor);
                PartyBase strikerTroopParty = strikerSide.GetAllocatedTroopParty(strikerTroopDescriptor);

                UniqueTroopDescriptor strikedTroopDescriptor = SelectSimulationTroopAtIndex(strikedSide, index);
                CharacterObject strikedTroop = strikedSide.GetAllocatedTroop(strikedTroopDescriptor);
                PartyBase strikedTroopParty = strikedSide.GetAllocatedTroopParty(strikedTroopDescriptor);

                float actualOffenseRating = distributedOffenseRating;
                if (__instance.IsPlayerSimulation && strikedTroopParty == PartyBase.MainParty)
                {
                    float damageMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
                    actualOffenseRating *= damageMultiplier;
                }
                DamageTypes damageType = (double)MBRandom.RandomFloat < 0.300000011920929 ? DamageTypes.Blunt : DamageTypes.Cut;
                float defenceRating = strikedTroop.GetPower();
                float damage = MBRandom.RandomFloat * 50f * actualOffenseRating / defenceRating;
                totalDamageDone += damage;
                /*
                internal enum SimulationTroopState
                {
                    Alive,
                    Wounded,
                    Killed,
                    Routed,
                }
                */
                // MapEvent.SimulationTroopState troopState;  
                // side2.ApplySimulationDamageToSelectedTroop(damage, damageType, out troopState, allocatedTroopParty1);
                object[] parametersArray = new object[] { (int)Math.Round(damage), damageType, null, strikerTroopParty };
                MapEventSide_ApplySimulationDamageToSelectedTroop.Invoke(strikedSide, parametersArray);
                int troopState = (int)parametersArray[2];

                // bool isFinishingStrike = troopState == MapEvent.SimulationTroopState.Killed || troopState == MapEvent.SimulationTroopState.Wounded;
                bool isFinishingStrike = troopState == 2 || troopState == 1;
                strikerSide.ApplySimulatedHitRewardToSelectedTroop(strikedTroop, 0, isFinishingStrike);
                finishedAnyone = finishedAnyone || isFinishingStrike;
            }

            return finishedAnyone;
        }

        static bool Prefix(ref bool __result,
            MapEvent __instance,
            int strikerSideIndex,
            int strikedSideIndex,
            float strikerAdvantage)
        {
            MapEventSide AttackerSide = __instance.AttackerSide;
            MapEventSide DefenderSide = __instance.DefenderSide;

            float AttackerTotalOffenseRating = AttackerSide.RecalculateStrengthOfSide();
            float DefenderTotalOffenseRating = DefenderSide.RecalculateStrengthOfSide();
            int AttackerNumber = AttackerSide.NumRemainingSimulationTroops;
            int DefenderNumber = DefenderSide.NumRemainingSimulationTroops;

            float AttackerDistributedOffenseRating = AttackerTotalOffenseRating / DefenderNumber * strikerAdvantage * 0.1f;
            float DefenderDistributedOffenseRating = DefenderTotalOffenseRating / AttackerNumber * 1.0f * 0.1f;

            bool finishedAnyone = false;
            float AttackerTotalDamageDone, DefenderTotalDamageDone;
            finishedAnyone |= StrikeOnce(__instance, AttackerSide, DefenderSide, AttackerDistributedOffenseRating, out AttackerTotalDamageDone);
            finishedAnyone |= StrikeOnce(__instance, DefenderSide, AttackerSide, DefenderDistributedOffenseRating, out DefenderTotalDamageDone);

            // Distribute XP among all living
            int AttackerAverageDamageDone = (int)Math.Round(Math.Min(AttackerTotalDamageDone / AttackerNumber, 1));
            for (int index = 0; index < AttackerSide.NumRemainingSimulationTroops; index++)
            {
                SelectSimulationTroopAtIndex(AttackerSide, index);
                AttackerSide.ApplySimulatedHitRewardToSelectedTroop(null, AttackerAverageDamageDone, false);
            }
            int DefenderAverageDamageDone = (int)Math.Round(Math.Min(DefenderTotalDamageDone / DefenderNumber, 1));
            for (int index = 0; index < AttackerSide.NumRemainingSimulationTroops; index++)
            {
                SelectSimulationTroopAtIndex(AttackerSide, index);
                AttackerSide.ApplySimulatedHitRewardToSelectedTroop(null, DefenderAverageDamageDone, false);
            }

            __result = finishedAnyone;

            return false;
        }


        static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops;
        }

    }

    [HarmonyPatch(typeof(MapEvent), "SimulateBattleForRounds")]
    public class SimulateBattleForRoundsPatch
    {
        static void Prefix(MapEvent __instance,
            ref int simulationRoundsDefender, ref int simulationRoundsAttacker)
        {
            int numAttackers = __instance.AttackerSide.NumRemainingSimulationTroops;
            int numDefenders = __instance.DefenderSide.NumRemainingSimulationTroops;
            double ratio1 = (Math.Pow(numAttackers, -0.6) + Math.Pow(numDefenders, -0.6));
            double ratio2 = __instance.IsSiegeAssault ? 0.2 : 1.0;
            int rounds = (int)Math.Round(Math.Max(ratio1 * ratio2 * 20f * SubModule.Settings.Battle_SendAllTroops_CombatSpeed, 1));
            simulationRoundsDefender = 0; // rounds;
            simulationRoundsAttacker = rounds;
        }

        static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops;
        }
    }

    [HarmonyPatch(typeof(MapEvent), "GetAttackersRunAwayChancePatch")]
    public class GetAttackersRunAwayChancePatch
    {
        static bool Prefix(ref bool __result, MapEvent __instance)
        {
            if (__instance.AttackerSide.LeaderParty.LeaderHero == null || __instance.IsSallyOut)
                return false;
            float num1 = 0.0f;
            foreach (PartyBase party in (IEnumerable<PartyBase>)__instance.AttackerSide.Parties)
                num1 += party.TotalStrength;
            float num2 = 0.0f;
            foreach (PartyBase party in (IEnumerable<PartyBase>)__instance.DefenderSide.Parties)
                num2 += party.TotalStrength;
            // if (__instance.IsSiege) v1.2.1
            if (__instance.IsSiegeAssault)         
                num1 *= 0.6666667f;
            float powerRatio = num2 / num1;

            bool AttackerRunaway = false;
            // Attacker Runaway
            if (powerRatio > 1.05)
            {
                float baseChance = (powerRatio - 1.05f) / 0.5f;
                AttackerRunaway = MBRandom.RandomFloat < baseChance;
            }

            
            bool DefenderRunaway = false;
            /*
            // Defender Runaway
            if (powerRatio <= 0.8 && __instance.DefenderSide.LeaderParty.LeaderHero != null && !__instance.IsSiegeAssault)
            {
                __instance.DefenderSide.MakeReadyForSimulation();
                this.RemoveRandomTroops(num, losses);
            }
            private void SacrifaceTroopsWithRatio(
                MobileParty mobileParty,
                float sacrifaceRatio,
                TroopRoster sacrifiedTroops)
            {
                int num1 = MBRandom.RoundRandomized((float)mobileParty.Party.NumberOfRegularMembers * sacrifaceRatio);
                for (int index = 0; index < num1; ++index)
                {
                    float num2 = 100f;
                    TroopRosterElement troopRosterElement1 = mobileParty.Party.MemberRoster.First<TroopRosterElement>();
                    foreach (TroopRosterElement troopRosterElement2 in mobileParty.Party.MemberRoster)
                    {
                        float num3 = (float)((double)troopRosterElement2.Character.Level - (troopRosterElement2.WoundedNumber > 0 ? 0.5 : 0.0) - (double)MBRandom.RandomFloat * 0.5);
                        if (!troopRosterElement2.Character.IsHero && (double)num3 < (double)num2 && troopRosterElement2.Number > 0)
                        {
                            num2 = num3;
                            troopRosterElement1 = troopRosterElement2;
                        }
                    }
                    mobileParty.MemberRoster.AddToCounts(troopRosterElement1.Character, -1, false, troopRosterElement1.WoundedNumber > 0 ? -1 : 0, 0, true, -1);
                    sacrifiedTroops.AddToCounts(troopRosterElement1.Character, 1, false, 0, 0, true, -1);
                }
            }
            */
            __result = AttackerRunaway || DefenderRunaway;

            return false;
        }

        static bool Prepare()
        {
            return SubModule.Settings.Strategy_LearnToQuit;
        }
    }
}
