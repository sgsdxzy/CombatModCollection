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
            TroopTemplate template = TroopTemplate.GetTroopTemplate(__instance);
            __result = template.Strength;

            return false;
        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_SendAllTroops && Settings.Instance.Battle_SendAllTroops_DetailedCombatModel;
        }
    }
}
