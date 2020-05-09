using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(Mission), "CreateBlow")]
    public class CreateBlowPatch
    {
        public static void Postfix(ref Blow __result,
            Agent attackerAgent,
            Agent victimAgent,
            ref AttackCollisionData collisionData,
            CrushThroughState cts,
            Vec3 blowDir,
            Vec3 swingDir,
            bool cancelDamage)
        {
            if ((__result.BlowFlag & BlowFlags.MakesRear) == BlowFlags.MakesRear)
            {
                __result.InflictedDamage = (int)(__result.InflictedDamage * SubModule.Settings.Battle_WarStomp_DamageMultiplierToHorse);
                if (SubModule.Settings.Battle_WarStomp_UnstoppableCharge)
                {
                    __result.BlowFlag &= ~BlowFlags.MakesRear;
                }
            }

            if (collisionData.IsHorseCharge && SubModule.Settings.Battle_WarStomp_WarStompDamageMultiplier != 1f)
            {
                if (victimAgent.IsRunningAway || (double)Vec3.DotProduct(swingDir, victimAgent.Frame.rotation.f) > 0.5)
                {
                    __result.InflictedDamage = (int)(__result.InflictedDamage * SubModule.Settings.Battle_WarStomp_WarStompDamageMultiplier);
                }
            }
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_WarStomp;
        }
    }
}
