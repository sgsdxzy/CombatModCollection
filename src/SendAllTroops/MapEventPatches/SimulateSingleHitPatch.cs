using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CombatModCollection.SendAllTroops.MapEventPatches
{
    [HarmonyPatch(typeof(MapEvent), "SimulateSingleHit")]
    public class SimulateSingleHitPatch
    {
        private static readonly PropertyInfo MapEvent_BattleObserver = typeof(MapEvent).GetProperty(
            "BattleObserver", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly float DamageMultiplier = 16.67f;
        private static readonly float RangedAverageDamagePerHit = 6.0f;

        private static bool StrikeOnce(MapEvent mapEvent,
            IBattleObserver battleObserver,
            MapEventSide strikerSide,
            MapEventSide strikedSide,
            AttackComposition attack,
            out float totalDamageDone)
        {
            int strikerNumber = strikerSide.NumRemainingSimulationTroops;
            int strikedNumber = strikedSide.NumRemainingSimulationTroops;
            totalDamageDone = 0;
            if (strikerNumber == 0 || strikedNumber == 0)
            {
                return true;
            }

            MapEventState mapEventState = MapEventState.GetMapEventState(mapEvent);
            bool finishedAnyone = false;
            for (int index = strikedNumber - 1; index >= 0; index--)
            {
                UniqueTroopDescriptor strikerTroopDescriptor = strikerSide.SelectRandomSimulationTroop();
                CharacterObject strikerTroop = strikerSide.GetAllocatedTroop(strikerTroopDescriptor);
                PartyBase strikerTroopParty = strikerSide.GetAllocatedTroopParty(strikerTroopDescriptor);

                UniqueTroopDescriptor strikedTroopDescriptor = MapEventSideHelper.SelectSimulationTroopAtIndex(strikedSide, index, out List<UniqueTroopDescriptor> strikedTroopList);
                CharacterObject strikedTroop = strikedSide.GetAllocatedTroop(strikedTroopDescriptor);
                PartyBase strikedTroopParty = strikedSide.GetAllocatedTroopParty(strikedTroopDescriptor);

                // MapEvents.GetSimulatedDamage and CombatSimulationModel.SimulateHit
                if (mapEvent.IsPlayerSimulation && strikedTroopParty == PartyBase.MainParty)
                {
                    float damageMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
                    attack *= damageMultiplier;
                }
                DamageTypes damageType = (double)MBRandom.RandomFloat < 0.15 ? DamageTypes.Blunt : DamageTypes.Cut;

                bool isFinishingStrike = MapEventSideHelper.ApplySimulationDamageToSelectedTroop(
                    strikedSide, strikedTroop, strikedTroopParty, strikedTroopDescriptor, index, strikedTroopList,
                    attack, damageType, strikerTroopParty, mapEventState, battleObserver, out float damage);
                totalDamageDone += damage;

                strikerSide.ApplySimulatedHitRewardToSelectedTroop(strikedTroop, 0, isFinishingStrike);
                finishedAnyone = finishedAnyone || isFinishingStrike;
            }

            return finishedAnyone;
        }

        public static bool Prefix(ref bool __result,
            MapEvent __instance,
            float strikerAdvantage)
        {
            MapEventSide attackerSide = __instance.AttackerSide;
            MapEventSide defenderSide = __instance.DefenderSide;
            AttackComposition attackerTotalAttack = new AttackComposition();
            AttackComposition defenderTotalAttack = new AttackComposition();

            MapEventState mapEventState = MapEventState.GetMapEventState(__instance);
            IBattleObserver battleObserver = (IBattleObserver)MapEvent_BattleObserver.GetValue(__instance);

            int attackerNumber = attackerSide.NumRemainingSimulationTroops;
            int defenderNumber = defenderSide.NumRemainingSimulationTroops;

            float strengthOfNumber;
            if (mapEventState.IsSiege)
            {
                if (Settings.Instance.Battle_SendAllTroops_DetailedCombatModel)
                {
                    if (mapEventState.GateBreached)
                    {
                        strengthOfNumber = (Settings.Instance.Battle_SendAllTroops_SiegeStrengthOfNumber
                            + Settings.Instance.Battle_SendAllTroops_StrengthOfNumber) / 2;
                    }
                    else
                    {
                        strengthOfNumber = Settings.Instance.Battle_SendAllTroops_SiegeStrengthOfNumber;
                    }
                }
                else
                {
                    strengthOfNumber = Settings.Instance.Battle_SendAllTroops_SiegeStrengthOfNumber;
                }
            }
            else
            {
                strengthOfNumber = Settings.Instance.Battle_SendAllTroops_StrengthOfNumber;
            }
            float battleSpeedMultiplier = DamageMultiplier;
            if (strengthOfNumber != 0.6f)
            {
                // Normalized battle speed to that of 0.6
                double biggerPartyNumber = Math.Max(attackerNumber, defenderNumber);
                battleSpeedMultiplier *= (float)Math.Pow(biggerPartyNumber, 0.6 - strengthOfNumber);
            }

            float attackerNumberPenalty = (float)Math.Pow((double)attackerNumber, strengthOfNumber - 1.0);
            float defenderNumberPenalty = (float)Math.Pow((double)defenderNumber, strengthOfNumber - 1.0);

            foreach (var party in attackerSide.Parties)
            {
                attackerTotalAttack += mapEventState.MakePartyAttack(party, battleSpeedMultiplier * attackerNumberPenalty / RangedAverageDamagePerHit);
            }
            foreach (var party in defenderSide.Parties)
            {
                defenderTotalAttack += mapEventState.MakePartyAttack(party, battleSpeedMultiplier * defenderNumberPenalty / RangedAverageDamagePerHit);
            }

            float attackerAdvantage = BattleAdvantageModel.PartyBattleAdvantage(attackerSide.LeaderParty);
            float defenderAdvantage = BattleAdvantageModel.PartyBattleAdvantage(defenderSide.LeaderParty);

            AttackComposition attackerDistributedAttack = attackerTotalAttack
                * (battleSpeedMultiplier * attackerNumberPenalty * attackerAdvantage * mapEventState.SettlementPenalty / defenderNumber);
            AttackComposition defenderDistributedAttack = defenderTotalAttack
                * (battleSpeedMultiplier * defenderNumberPenalty * defenderAdvantage / attackerNumber);

            bool finishedAnyone = false;
            finishedAnyone |= StrikeOnce(__instance, battleObserver, attackerSide, defenderSide, attackerDistributedAttack, out float attackerTotalDamageDone);
            finishedAnyone |= StrikeOnce(__instance, battleObserver, defenderSide, attackerSide, defenderDistributedAttack, out float defenderTotalDamageDone);

            // Distribute XP among all living
            int AttackerAverageDamageDone = (int)Math.Round(Math.Min(attackerTotalDamageDone / attackerNumber, 1));
            try
            {
                for (int index = 0; index < attackerSide.NumRemainingSimulationTroops; index++)
                {
                    MapEventSideHelper.SelectSimulationTroopAtIndex(attackerSide, index, out _);
                    attackerSide.ApplySimulatedHitRewardToSelectedTroop(null, AttackerAverageDamageDone, false);
                }
                int DefenderAverageDamageDone = (int)Math.Round(Math.Min(defenderTotalDamageDone / defenderNumber, 1));
                for (int index = 0; index < attackerSide.NumRemainingSimulationTroops; index++)
                {
                    MapEventSideHelper.SelectSimulationTroopAtIndex(attackerSide, index, out _);
                    attackerSide.ApplySimulatedHitRewardToSelectedTroop(null, DefenderAverageDamageDone, false);
                }
            }
            catch (NullReferenceException)
            {
                // CombatXpModel.GetXpFromHit is changed by other mod and not accepting attackedTroop == null
            }

            __result = finishedAnyone;

            return false;
        }


        public static bool Prepare()
        {
            return Settings.Instance.Battle_SendAllTroops;
        }

    }
}