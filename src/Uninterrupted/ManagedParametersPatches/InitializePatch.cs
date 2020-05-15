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
                 Settings.Instance.Battle_Uninterrupted_DamageInterruptAttackthresholdPierce);
            ManagedParameters.SetParameter(ManagedParametersEnum.DamageInterruptAttackThresholdCut,
                 Settings.Instance.Battle_Uninterrupted_DamageInterruptAttackthresholdCut);
            ManagedParameters.SetParameter(ManagedParametersEnum.DamageInterruptAttackThresholdBlunt,
                 Settings.Instance.Battle_Uninterrupted_DamageInterruptAttackthresholdBlunt);
        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_Uninterrupted;
        }
    }
}
