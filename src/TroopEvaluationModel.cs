using TaleWorlds.CampaignSystem;

namespace CombatModCollection
{
    public class TroopEvaluationModel
    {
        public static float GetAttackPoints(CharacterObject troop, int StageRounds = 0)
        {
            if (SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                return GetAttackPointsNewModel(troop, StageRounds);
            } else
            {
                return troop.GetPower();
            }
            
        }

        private static float GetAttackPointsNewModel(CharacterObject troop, int StageRounds = 0)
        {
            troop.GetSimulationAttackPower(out float attackPoints, out _);
            return attackPoints;
        }

        public static float GetDefensePoints(CharacterObject troop, int StageRounds = 0)
        {
            if (SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                return GetDefensePointsNewModel(troop, StageRounds);
            } else
            {
                return troop.GetPower();
            }
        }

        private static float GetDefensePointsNewModel(CharacterObject troop, int StageRounds = 0)
        {
            troop.GetSimulationAttackPower(out _, out float defensePoints);
            return defensePoints;
        }
    }
}
