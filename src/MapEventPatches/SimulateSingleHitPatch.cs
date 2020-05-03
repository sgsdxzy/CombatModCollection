using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.Core;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(MapEvent), "SimulateSingleHit")]
    public class SimulateSingleHitPatch
    {
        // private static MethodInfo MapEventSide_ApplySimulationDamageToSelectedTroop = typeof(MapEventSide).GetMethod(
        //     "ApplySimulationDamageToSelectedTroop", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static PropertyInfo MapEvent_BattleObserver = typeof(MapEvent).GetProperty(
             "BattleObserver", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static FieldInfo MapEventSide__selectedSimulationTroopIndex = typeof(MapEventSide).GetField(
            "_selectedSimulationTroopIndex", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static FieldInfo MapEventSide__selectedSimulationTroopDescriptor = typeof(MapEventSide).GetField(
            "_selectedSimulationTroopDescriptor", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static FieldInfo MapEventSide__simulationTroopList = typeof(MapEventSide).GetField(
            "_simulationTroopList", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static FieldInfo MapEventSide__selectedSimulationTroop = typeof(MapEventSide).GetField(
            "_selectedSimulationTroop", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly float DamageMultiplier = 5.0f;

        private static UniqueTroopDescriptor SelectSimulationTroopAtIndex(MapEventSide side, int index, out List<UniqueTroopDescriptor> simulationTroopList)
        {
            // side._selectedSimulationTroopIndex = index;
            MapEventSide__selectedSimulationTroopIndex.SetValue(side, index);

            // side._selectedSimulationTroopDescriptor = side._simulationTroopList[index];
            simulationTroopList = (List<UniqueTroopDescriptor>)MapEventSide__simulationTroopList.GetValue(side);
            UniqueTroopDescriptor selectedSimulationTroopDescriptor = simulationTroopList[index];

            MapEventSide__selectedSimulationTroopDescriptor.SetValue(side, selectedSimulationTroopDescriptor);

            // side._selectedSimulationTroop = side.GetAllocatedTroop(side._selectedSimulationTroopDescriptor);
            MapEventSide__selectedSimulationTroop.SetValue(side, side.GetAllocatedTroop(selectedSimulationTroopDescriptor));

            return selectedSimulationTroopDescriptor;
        }

        // MapEventSide.RemoveSelectedTroopFromSimulationList
        private static void RemoveSelectedTroopFromSimulationList(MapEventSide side,
            int selectedSimulationTroopIndex,
            List<UniqueTroopDescriptor> simulationTroopList)
        {
            // this._simulationTroopList[this._selectedSimulationTroopIndex] = this._simulationTroopList[this._simulationTroopList.Count - 1];
            // this._simulationTroopList.RemoveAt(this._simulationTroopList.Count - 1);
            // int selectedSimulationTroopIndex = (int)MapEventSide__selectedSimulationTroopIndex.GetValue(side);
            // List<UniqueTroopDescriptor> simulationTroopList = (List<UniqueTroopDescriptor>)MapEventSide__simulationTroopList.GetValue(side);
            simulationTroopList[selectedSimulationTroopIndex] = simulationTroopList[simulationTroopList.Count - 1];
            simulationTroopList.RemoveAt(simulationTroopList.Count - 1);

            // this._selectedSimulationTroopIndex = -1;
            // this._selectedSimulationTroopDescriptor = UniqueTroopDescriptor.Invalid;
            // this._selectedSimulationTroop = (CharacterObject)null;
            MapEventSide__selectedSimulationTroopIndex.SetValue(side, -1);
            MapEventSide__selectedSimulationTroopDescriptor.SetValue(side, UniqueTroopDescriptor.Invalid);
            MapEventSide__selectedSimulationTroop.SetValue(side, (CharacterObject)null);
        }

        // MapEventSide.ApplySimulationDamageToSelectedTroop
        private static bool ApplySimulationDamageToSelectedTroop(MapEventSide side,
            CharacterObject strikedTroop,
            PartyBase strikedTroopParty,
            UniqueTroopDescriptor strikedTroopDescriptor,
            int selectedSimulationTroopIndex,
            List<UniqueTroopDescriptor> strikedTroopList,
            float damage,
            DamageTypes damageType,
            PartyBase strikerParty,
            MapEventStat mapEventStat,
            IBattleObserver battleObserver)
        {
            bool flag = false;
            if (strikedTroop.IsHero)
            {
                side.AddHeroDamage(strikedTroop.HeroObject, (int)Math.Round(damage));
                if (strikedTroop.HeroObject.IsWounded)
                {
                    flag = true;
                    battleObserver?.TroopNumberChanged(side.MissionSide, (IBattleCombatant)strikedTroopParty, (BasicCharacterObject)strikedTroop, -1, 0, 1, 0, 0, 0);
                }
            }
            else
            {
                if (!mapEventStat.TroopStats.TryGetValue(strikedTroop.Id, out TroopStat troopStat))
                {
                    troopStat = new TroopStat
                    {
                        Hitpoints = strikedTroop.MaxHitPoints()
                    };
                    mapEventStat.TroopStats[strikedTroop.Id] = troopStat;
                }
                troopStat.Hitpoints -= damage;

                if (troopStat.Hitpoints <= 0)
                {
                    if ((double)MBRandom.RandomFloat < (double)Campaign.Current.Models.PartyHealingModel.GetSurvivalChance(strikedTroopParty, strikedTroop, damageType, strikerParty))
                    {
                        side.OnTroopWounded(strikedTroopDescriptor);
                        battleObserver?.TroopNumberChanged(side.MissionSide, (IBattleCombatant)strikedTroopParty, (BasicCharacterObject)strikedTroop, -1, 0, 1, 0, 0, 0);
                        SkillLevelingManager.OnSurgeryApplied(strikedTroopParty.MobileParty, 1f);
                    }
                    else
                    {
                        side.OnTroopKilled(strikedTroopDescriptor);
                        battleObserver?.TroopNumberChanged(side.MissionSide, (IBattleCombatant)strikedTroopParty, (BasicCharacterObject)strikedTroop, -1, 1, 0, 0, 0, 0);
                        SkillLevelingManager.OnSurgeryApplied(strikedTroopParty.MobileParty, 0.5f);
                    }
                    flag = true;
                    mapEventStat.TroopStats.TryRemove(strikedTroop.Id, out _);
                }
            }
            if (flag)
                // side.RemoveSelectedTroopFromSimulationList();
                RemoveSelectedTroopFromSimulationList(side, selectedSimulationTroopIndex, strikedTroopList);
            return flag;
        }

        private static bool StrikeOnce(MapEvent mapEvent,
            IBattleObserver battleObserver,
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
            MapEventStat mapEventStat = GlobalStorage.MapEventStats[mapEvent.Id];

            bool finishedAnyone = false;
            for (int index = strikedNumber - 1; index >= 0; index--)
            {
                UniqueTroopDescriptor strikerTroopDescriptor = strikerSide.SelectRandomSimulationTroop();
                CharacterObject strikerTroop = strikerSide.GetAllocatedTroop(strikerTroopDescriptor);
                PartyBase strikerTroopParty = strikerSide.GetAllocatedTroopParty(strikerTroopDescriptor);

                List<UniqueTroopDescriptor> strikedTroopList;
                UniqueTroopDescriptor strikedTroopDescriptor = SelectSimulationTroopAtIndex(strikedSide, index, out strikedTroopList);
                CharacterObject strikedTroop = strikedSide.GetAllocatedTroop(strikedTroopDescriptor);
                PartyBase strikedTroopParty = strikedSide.GetAllocatedTroopParty(strikedTroopDescriptor);

                // MapEvents.GetSimulatedDamage and CombatSimulationModel.SimulateHit
                float actualOffenseRating = distributedOffenseRating;
                if (mapEvent.IsPlayerSimulation && strikedTroopParty == PartyBase.MainParty)
                {
                    float damageMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
                    actualOffenseRating *= damageMultiplier;
                }
                DamageTypes damageType = (double)MBRandom.RandomFloat < 0.300000011920929 ? DamageTypes.Blunt : DamageTypes.Cut;
                float defenceRating = strikedTroop.GetPower();
                float damage = DamageMultiplier * actualOffenseRating / defenceRating;
                if (SubModule.Settings.Battle_SendAllTroops_AbsoluteZeroRandomness)
                {
                    damage *= 0.5f;
                } else
                {
                    damage *= MBRandom.RandomFloat;
                }
                totalDamageDone += damage;

                bool isFinishingStrike = ApplySimulationDamageToSelectedTroop(strikedSide, strikedTroop, strikedTroopParty,
                    strikedTroopDescriptor, index, strikedTroopList, damage, damageType, strikerTroopParty, mapEventStat, battleObserver);

                strikerSide.ApplySimulatedHitRewardToSelectedTroop(strikedTroop, 0, isFinishingStrike);
                finishedAnyone = finishedAnyone || isFinishingStrike;
            }

            return finishedAnyone;
        }

        public static bool Prefix(ref bool __result,
            MapEvent __instance,
            float strikerAdvantage)
        {
            MapEventSide AttackerSide = __instance.AttackerSide;
            MapEventSide DefenderSide = __instance.DefenderSide;
            IBattleObserver battleObserver = (IBattleObserver)MapEvent_BattleObserver.GetValue(__instance);

            float AttackerTotalOffenseRating = AttackerSide.RecalculateStrengthOfSide();
            float DefenderTotalOffenseRating = DefenderSide.RecalculateStrengthOfSide();
            int AttackerNumber = AttackerSide.NumRemainingSimulationTroops;
            int DefenderNumber = DefenderSide.NumRemainingSimulationTroops;

            float AttackerDistributedOffenseRating = AttackerTotalOffenseRating / DefenderNumber * strikerAdvantage;
            float DefenderDistributedOffenseRating = DefenderTotalOffenseRating / AttackerNumber * 1.0f;

            bool finishedAnyone = false;
            finishedAnyone |= StrikeOnce(__instance, battleObserver, AttackerSide, DefenderSide, AttackerDistributedOffenseRating, out float AttackerTotalDamageDone);
            finishedAnyone |= StrikeOnce(__instance, battleObserver, DefenderSide, AttackerSide, DefenderDistributedOffenseRating, out float DefenderTotalDamageDone);

            // Distribute XP among all living
            int AttackerAverageDamageDone = (int)Math.Round(Math.Min(AttackerTotalDamageDone / AttackerNumber, 1));
            try
            {
                for (int index = 0; index < AttackerSide.NumRemainingSimulationTroops; index++)
                {
                    SelectSimulationTroopAtIndex(AttackerSide, index, out _);
                    AttackerSide.ApplySimulatedHitRewardToSelectedTroop(null, AttackerAverageDamageDone, false);
                }
                int DefenderAverageDamageDone = (int)Math.Round(Math.Min(DefenderTotalDamageDone / DefenderNumber, 1));
                for (int index = 0; index < AttackerSide.NumRemainingSimulationTroops; index++)
                {
                    SelectSimulationTroopAtIndex(AttackerSide, index, out _);
                    AttackerSide.ApplySimulatedHitRewardToSelectedTroop(null, DefenderAverageDamageDone, false);
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
            return SubModule.Settings.Battle_SendAllTroops;
        }

    }
}