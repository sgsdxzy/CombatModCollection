using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CombatModCollection.SendAllTroops.CharacterObjectPatches
{
    [HarmonyPatch(typeof(CharacterObject), "GetPower")]
    public class GetPowerPatch
    {
        public static bool Prefix(ref float __result,
            CharacterObject __instance)
        {
            if (SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                TroopTemplate template = TroopTemplate.GetTroopTemplate(__instance);
                __result = template.Strength;

                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops;
        }
    }
}
