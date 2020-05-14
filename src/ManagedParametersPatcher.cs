using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(MissionState), "FinishMissionLoading")]
    public class ManagedParametersPatcher
    {
        public static void Postfix()
        {
            if (Settings.Instance.Battle_RealisticBallistics)
            {
                ManagedParameters.SetParameter(ManagedParametersEnum.AirFrictionArrow,
                     Settings.Instance.Battle_RealisticBallistics_AirFrictionArrow);
                ManagedParameters.SetParameter(ManagedParametersEnum.AirFrictionJavelin,
                    Settings.Instance.Battle_RealisticBallistics_AirFrictionJavelin);
                ManagedParameters.SetParameter(ManagedParametersEnum.AirFrictionAxe,
                     Settings.Instance.Battle_RealisticBallistics_AirFrictionAxe);
                ManagedParameters.SetParameter(ManagedParametersEnum.AirFrictionKnife,
                     Settings.Instance.Battle_RealisticBallistics_AirFrictionKnife);
            }

            if (Settings.Instance.Battle_PowerThrust)
            {
                ManagedParameters.SetParameter(ManagedParametersEnum.NonTipThrustHitDamageMultiplier,
                    Settings.Instance.Battle_PowerThrust_NonTipThrustHitDamageMultiplier);
                ManagedParameters.SetParameter(ManagedParametersEnum.ThrustHitWithArmDamageMultiplier,
                    Settings.Instance.Battle_PowerThrust_ThrustHitWithArmDamageMultiplier);
                ManagedParameters.SetParameter(ManagedParametersEnum.ThrustCombatSpeedGraphZeroProgressValue, 1.0f);
            }

        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_PowerThrust || Settings.Instance.Battle_RealisticBallistics;
        }
    }
}
