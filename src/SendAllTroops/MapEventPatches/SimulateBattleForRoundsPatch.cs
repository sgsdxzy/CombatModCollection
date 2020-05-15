using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;

namespace CombatModCollection.SendAllTroops.MapEventPatches
{
    [HarmonyPatch(typeof(MapEvent), "SimulateBattleForRounds")]
    public class SimulateBattleForRoundsPatch
    {
        public static readonly float RoundsPrecision = 3;
        public static void Prefix(MapEvent __instance,
            ref int simulationRoundsDefender, ref int simulationRoundsAttacker)
        {
            double rounds = RoundsPrecision * Settings.Instance.Battle_SendAllTroops_CombatSpeed;
            if (__instance.IsSiegeAssault)
            {
                rounds *= 0.4f;
            }
            rounds = Math.Max(rounds, 1);
            simulationRoundsDefender = 0;
            simulationRoundsAttacker = (int)Math.Round(rounds);
        }

        public static void Postfix(MapEvent __instance)
        {
            MapEventState mapEventState = MapEventState.GetMapEventState(__instance);
            mapEventState.StageRounds += 1;
        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_SendAllTroops;
        }
    }
}