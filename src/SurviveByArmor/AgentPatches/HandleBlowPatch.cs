﻿using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection.SurviveByArmor.AgentPatches
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
                    BasicCharacterObjectCustomMembers.ExcessiveDamages[__instance.Character.Id] = excessiveDamage;
                }
            }
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SurviveByArmor;
        }
    }
}
