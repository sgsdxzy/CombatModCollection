using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(Agent), "HandleBlow")]
    public class HandleBlowPatch
    {
        public static void Prefix(ref Blow b, Agent __instance)
        {
            float excessiveDamage = (float)b.InflictedDamage - __instance.Health + 1f;
            if (excessiveDamage >= 0 && b.DamageType != DamageTypes.Blunt)
            {
                if (MBRandom.RandomFloat < SurvivalModel.GetExcessiveDamageSurvivalChance(__instance, excessiveDamage))
                {
                    b.DamageType = DamageTypes.Blunt;
                }            
            }
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SurviveByArmor && SubModule.Settings.Battle_SurviveByArmor_SurviveByExcessiveDamage;
        }
    }
}
