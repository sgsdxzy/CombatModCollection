using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CombatModCollection.SendAllTroops.MapStatePatches
{
    [HarmonyPatch(typeof(MapState), "EndBattleSimulation")]
    public class EndBattleSimulationPatch
    {
        public static void Postfix()
        {
            MapEventState.RemoveMapEventState(PlayerEncounter.Battle);
        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_SendAllTroops;
        }
    }
}
