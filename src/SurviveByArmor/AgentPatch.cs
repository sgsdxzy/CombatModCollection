using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(Agent), "HandleBlow")]
    public class HandleBlowPatch
    {
        public static void Prefix(ref Blow b, Agent __instance)
        {
            if (__instance.Character != null)
            {
                float excessiveDamage = (float)b.InflictedDamage - __instance.Health + 1f;
                if (excessiveDamage > 0)
                {
                    GetSurvivalChancePatch.ExcessiveDamages[__instance.Character.Id] = excessiveDamage;
                }
            }
        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_SurviveByArmor;
        }
    }
}
