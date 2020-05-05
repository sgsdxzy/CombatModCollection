﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(MapEvent), "SimulateSingleHit")]
    public class SimulateSingleHitPatch
    {
        private static readonly PropertyInfo MapEvent_BattleObserver = typeof(MapEvent).GetProperty(
            "BattleObserver", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly float DamageMultiplier = 10;

        private static bool StrikeOnce(MapEvent mapEvent,
            IBattleObserver battleObserver,
            MapEventSide strikerSide,
            MapEventSide strikedSide,
            AttackComposition distributedAttackPoints,
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
                AttackComposition actualAttackPoints = distributedAttackPoints;
                if (mapEvent.IsPlayerSimulation && strikedTroopParty == PartyBase.MainParty)
                {
                    float damageMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
                    actualAttackPoints *= damageMultiplier;
                }               
                if (SubModule.Settings.Battle_SendAllTroops_AbsoluteZeroRandomness)
                {
                    actualAttackPoints *= 0.5f;
                }
                else
                {
                    actualAttackPoints *= MBRandom.RandomFloat;
                }
                DamageTypes damageType = (double)MBRandom.RandomFloat < 0.300000011920929 ? DamageTypes.Blunt : DamageTypes.Cut;

                bool isFinishingStrike = MapEventSideHelper.ApplySimulationDamageToSelectedTroop(
                    strikedSide, strikedTroop, strikedTroopParty, strikedTroopDescriptor, index, strikedTroopList, 
                    actualAttackPoints, damageType, strikerTroopParty, mapEventState, battleObserver, out float damage);
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
            AttackComposition attackerTotalAttackPoints = new AttackComposition();
            AttackComposition defenderTotalAttackPoints = new AttackComposition();
            MapEventState mapEventState = MapEventState.GetMapEventState(__instance);

            IBattleObserver battleObserver = (IBattleObserver)MapEvent_BattleObserver.GetValue(__instance);

            for (int index = 0; index < attackerSide.NumRemainingSimulationTroops; index++)
            {
                UniqueTroopDescriptor troopDescriptor = MapEventSideHelper.SelectSimulationTroopAtIndex(attackerSide, index, out _);
                CharacterObject troop = attackerSide.GetAllocatedTroop(troopDescriptor);
                attackerTotalAttackPoints += mapEventState.GetAttackPoints(troop);
            }
            for (int index = 0; index < defenderSide.NumRemainingSimulationTroops; index++)
            {
                UniqueTroopDescriptor troopDescriptor = MapEventSideHelper.SelectSimulationTroopAtIndex(defenderSide, index, out _);
                CharacterObject troop = defenderSide.GetAllocatedTroop(troopDescriptor);
                defenderTotalAttackPoints += mapEventState.GetAttackPoints(troop);
            }

            int attackerNumber = attackerSide.NumRemainingSimulationTroops;
            int defenderNumber = defenderSide.NumRemainingSimulationTroops;
            AttackComposition attackerDistributedAttackPoints = attackerTotalAttackPoints * DamageMultiplier / defenderNumber * strikerAdvantage;
            AttackComposition defenderDistributedAttackPoints = defenderTotalAttackPoints * DamageMultiplier / attackerNumber * 1.0f;

            bool finishedAnyone = false;
            finishedAnyone |= StrikeOnce(__instance, battleObserver, attackerSide, defenderSide, attackerDistributedAttackPoints, out float attackerTotalDamageDone);
            finishedAnyone |= StrikeOnce(__instance, battleObserver, defenderSide, attackerSide, defenderDistributedAttackPoints, out float defenderTotalDamageDone);

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
            return SubModule.Settings.Battle_SendAllTroops;
        }

    }
}