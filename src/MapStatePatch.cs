using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(MapState), "EndBattleSimulation")]
    public class EndBattleSimulationPatch
    {
        public static void Postfix()
        {
            GlobalStorage.MapEventStats.TryRemove(PlayerEncounter.Battle.Id, out _);
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops;
        }
    }
}
