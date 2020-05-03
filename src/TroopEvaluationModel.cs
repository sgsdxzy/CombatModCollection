using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem;

namespace CombatModCollection
{
    public class TroopEvaluationModel
    {
        public static float GetAttackPoints(CharacterObject troop)
        {
            if (SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                troop.GetSimulationAttackPower(out float attackPoints, out _);
                return attackPoints;
            } else
            {
                return troop.GetPower();
            }
            
        }

        public static float GetDefensePoints(CharacterObject troop)
        {
            if (SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                troop.GetSimulationAttackPower(out _, out float defensePoints);
                return defensePoints;
            } else
            {
                return troop.GetPower();
            }
        }
    }
}
