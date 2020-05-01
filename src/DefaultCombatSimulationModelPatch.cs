using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(DefaultCombatSimulationModel), "GetBattleAdvantage")]
    public class GetBattleAdvantagePatch
    {        
        public static void Postfix(ref (float defenderAdvantage, float attackerAdvantage) __result)
        {
            // if (__result.defenderAdvantage <= 1)
            // {
            //     return;
            // }
            __result.attackerAdvantage /= __result.defenderAdvantage;
            __result.defenderAdvantage = 1.0f;
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops;
        }
    }
}
