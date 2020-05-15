using HarmonyLib;
using TaleWorlds.Core;

namespace CombatModCollection.PowerThrust.ManagedParametersPatches
{
    [HarmonyPatch(typeof(ManagedParameters), "Initialize")]
    public class InitializePatch
    {
        public static void Postfix()
        {
            ManagedParameters.SetParameter(ManagedParametersEnum.NonTipThrustHitDamageMultiplier,
                Settings.Instance.Battle_PowerThrust_NonTipThrustHitDamageMultiplier);
            ManagedParameters.SetParameter(ManagedParametersEnum.ThrustHitWithArmDamageMultiplier,
                Settings.Instance.Battle_PowerThrust_ThrustHitWithArmDamageMultiplier);
            ManagedParameters.SetParameter(ManagedParametersEnum.ThrustCombatSpeedGraphZeroProgressValue, 1.0f);
        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_PowerThrust;
        }
    }
}
