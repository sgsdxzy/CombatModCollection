using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(MapEvent), "SimulateBattleForRounds")]
    public class SimulateBattleForRoundsPatch
    {
        public static void Prefix(MapEvent __instance, int ____mapEventUpdateCount,
            ref int simulationRoundsDefender, ref int simulationRoundsAttacker)
        {
            int numAttackers = __instance.AttackerSide.NumRemainingSimulationTroops;
            int numDefenders = __instance.DefenderSide.NumRemainingSimulationTroops;
            double ratio1 = (Math.Pow(numAttackers, -0.6) + Math.Pow(numDefenders, -0.6));
            double ratio2 = __instance.IsSiegeAssault ? 0.2 : 1.0;
            int rounds = (int)Math.Round(Math.Max(ratio1 * ratio2 * 40f * SubModule.Settings.Battle_SendAllTroops_CombatSpeed, 1));
            simulationRoundsDefender = 0; // rounds;
            simulationRoundsAttacker = 1; // rounds;

            if (!GlobalStorage.MapEventStats.ContainsKey(__instance.Id))
            {
                GlobalStorage.MapEventStats[__instance.Id] = new MapEventStat();
                GlobalStorage.MapEventStats[__instance.Id].StageRounds = ____mapEventUpdateCount;
            }
            else
            {
                GlobalStorage.MapEventStats[__instance.Id].StageRounds += 1;
            }
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops;
        }
    }
}