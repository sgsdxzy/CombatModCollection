using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(CharacterObject), "GetPower")]
    public class GetPowerPatch
    {
        public static bool Prefix(ref float __result,
            CharacterObject __instance)
        {
            __result = 4.68e-7f * TroopEvaluationModel.GetAttackPoints(__instance) * TroopEvaluationModel.GetDefensePoints(__instance);

            return false;
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops && SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel;
        }
    }

    // Probably should change CalculateStrength()
}
