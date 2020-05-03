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

            troop.GetSimulationAttackPower(out float attackPoints, out _);
            return attackPoints;
        }

        public static float GetDefensePoints(CharacterObject troop)
        {

            troop.GetSimulationAttackPower(out _, out float defensePoints);
            return defensePoints;
        }
    }
}
