using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection.WarStomp.MissionPatches
{
    [HarmonyPatch(typeof(Mission), "ComputeBlowMagnitudeFromHorseCharge")]
    public class ComputeBlowMagnitudeFromHorseChargePatch
    {
        private const double NonZero = 1e-5;

        public static void Postfix(ref AttackCollisionData acd,
            Vec3 attackerAgentMovementDirection,
            Vec3 attackerAgentVelocity,
            float agentMountChargeDamageProperty,
            Vec3 victimAgentVelocity,
            Vec3 victimAgentPosition,
            ref float baseMagnitude,
            ref float specialMagnitude)
        {
            if (attackerAgentVelocity.Length > NonZero && victimAgentVelocity.Length > NonZero)
            {
                if (Vec3.DotProduct(attackerAgentVelocity, victimAgentVelocity) /
                    (attackerAgentVelocity.Length * victimAgentVelocity.Length) > 0.5)
                {
                    specialMagnitude *= Settings.Instance.Battle_WarStomp_WarStompDamageMultiplier;
                }
            }

        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_WarStomp;
        }
    }
}
