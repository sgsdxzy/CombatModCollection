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
            // TroopState troopState = new TroopState(__instance);
            // __result = troopState.GetStrength(0);

            return false;
        }

        public static bool Prepare()
        {
            return false;
            return SubModule.Settings.Battle_SendAllTroops && SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel;
        }
    }

    // Probably should change CalculateStrength()
}
