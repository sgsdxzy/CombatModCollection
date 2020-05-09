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
            TroopTemplate template = TroopTemplate.GetTroopTemplate(__instance);
            __result = template.Strength;

            return false;
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops && SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel;
        }
    }
}
