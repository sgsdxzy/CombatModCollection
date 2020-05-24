using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CombatModCollection.SendAllTroops.MapEventPatches
{
    [HarmonyPatch(typeof(MapEvent), "FinishBattle")]
    public class FinishBattlePatch
    {
        public static void Postfix(MapEvent __instance)
        {
            MapEventState.RemoveMapEventState(__instance);
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops;
        }
    }
}