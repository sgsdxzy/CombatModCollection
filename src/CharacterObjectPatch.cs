using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(CharacterObject), "GetPower")]
    public class GetPowerPatch
    {
        public static bool Prefix(ref float __result,
            CharacterObject __instance)
        {
            __result = 4.68e-9f * TroopEvaluationModel.GetAttackPoints(__instance) * 
                TroopEvaluationModel.GetDefensePoints(__instance) *
                __instance.MaxHitPoints();

            InformationManager.DisplayMessage(new InformationMessage(__instance.Name.ToString() + 
                "  " + TroopEvaluationModel.GetAttackPoints(__instance).ToString()
                + "  " + TroopEvaluationModel.GetDefensePoints(__instance) 
                + "  " + __instance.MaxHitPoints()));

            return false;
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops && SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel;
        }
    }

    // Probably should change CalculateStrength()
}
