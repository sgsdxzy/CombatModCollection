using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.Core;

namespace CombatModCollection
{
    public class MapEventSideHelper
    {
        private static readonly FieldInfo MapEventSide__selectedSimulationTroopIndex = typeof(MapEventSide).GetField(
            "_selectedSimulationTroopIndex", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly FieldInfo MapEventSide__selectedSimulationTroopDescriptor = typeof(MapEventSide).GetField(
            "_selectedSimulationTroopDescriptor", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly FieldInfo MapEventSide__simulationTroopList = typeof(MapEventSide).GetField(
            "_simulationTroopList", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly FieldInfo MapEventSide__selectedSimulationTroop = typeof(MapEventSide).GetField(
            "_selectedSimulationTroop", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);


        public static UniqueTroopDescriptor SelectSimulationTroopAtIndex(MapEventSide side, int index, out List<UniqueTroopDescriptor> simulationTroopList)
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
        public static void RemoveSelectedTroopFromSimulationList(MapEventSide side,
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
        public static bool ApplySimulationDamageToSelectedTroop(MapEventSide side,
            CharacterObject strikedTroop,
            PartyBase strikedTroopParty,
            UniqueTroopDescriptor strikedTroopDescriptor,
            int selectedSimulationTroopIndex,
            List<UniqueTroopDescriptor> strikedTroopList,
            AttackComposition attackPoints,
            DamageTypes damageType,
            PartyBase strikerParty,
            MapEventState mapEventState,
            IBattleObserver battleObserver,
            out float damage)
        {
            bool flag = false;
            bool IsFinishingStrike = mapEventState.ApplyDamageToTroop(attackPoints, strikedTroop, mapEventState.StageRounds, out damage);
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
                if (IsFinishingStrike)
                {
                    float survivalChance = Campaign.Current.Models.PartyHealingModel.GetSurvivalChance(strikedTroopParty, strikedTroop, damageType, strikerParty);
                    if (MBRandom.RandomFloat < survivalChance)
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
                }
            }
            if (flag)
                // side.RemoveSelectedTroopFromSimulationList();
                RemoveSelectedTroopFromSimulationList(side, selectedSimulationTroopIndex, strikedTroopList);
            return flag;
        }

        public static float RecalculateStrengthOfSide(MapEventSide side, MapEventState mapEventState)
        {
            if (!SubModule.Settings.Battle_SendAllTroops)
            {
                return side.RecalculateStrengthOfSide();
            }
            else
            {
                float totalStrength = 0f;
                for (int index = 0; index < side.NumRemainingSimulationTroops; index++)
                {
                    UniqueTroopDescriptor troopDescriptor = SelectSimulationTroopAtIndex(side, index, out _);
                    CharacterObject troop = side.GetAllocatedTroop(troopDescriptor);
                    float strength = mapEventState.GetTroopStrength(troop);

                    totalStrength += strength;
                }
                return totalStrength;
            }
        }
    }
}
