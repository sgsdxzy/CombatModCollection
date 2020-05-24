using HarmonyLib;
using TaleWorlds.Core;

namespace CombatModCollection.Uninterrupted.ManagedParametersPatches
{
    [HarmonyPatch(typeof(ManagedParameters), "Initialize")]
    public class InitializePatch
    {
        public static void Postfix()
        {
            ManagedParameters.SetParameter(ManagedParametersEnum.DamageInterruptAttackThresholdPierce,
                 SubModule.Settings.Battle_Uninterrupted_DamageInterruptAttackthresholdPierce);
            ManagedParameters.SetParameter(ManagedParametersEnum.DamageInterruptAttackThresholdCut,
                 SubModule.Settings.Battle_Uninterrupted_DamageInterruptAttackthresholdCut);
            ManagedParameters.SetParameter(ManagedParametersEnum.DamageInterruptAttackThresholdBlunt,
                 SubModule.Settings.Battle_Uninterrupted_DamageInterruptAttackthresholdBlunt);
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_Uninterrupted;
        }
    }
}
