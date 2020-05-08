using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(MapEvent), "SimulateBattleForRounds")]
    public class SimulateBattleForRoundsPatch
    {
        public static void Prefix(MapEvent __instance,
            ref int simulationRoundsDefender, ref int simulationRoundsAttacker)
        {
            int numAttackers = __instance.AttackerSide.NumRemainingSimulationTroops;
            int numDefenders = __instance.DefenderSide.NumRemainingSimulationTroops;
            double ratio1 = (Math.Pow(numAttackers, -0.4) + Math.Pow(numDefenders, -0.4));
            double ratio2 = __instance.IsSiegeAssault ? 0.2 : 1.0;
            int rounds = (int)Math.Round(Math.Max(ratio1 * ratio2 * 5f * SubModule.Settings.Battle_SendAllTroops_CombatSpeed, 1));
            simulationRoundsDefender = 0; // rounds;
            simulationRoundsAttacker = 1; // rounds;

            MapEventState mapEventState = MapEventState.GetMapEventState(__instance);
            mapEventState.UpdateEventState(__instance);
        }

        public static void Postfix(MapEvent __instance)
        {
            MapEventState mapEventState = MapEventState.GetMapEventState(__instance);
            mapEventState.StageRounds += 1;
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops;
        }
    }
}