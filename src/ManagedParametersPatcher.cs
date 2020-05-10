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
            if (SubModule.Settings.Battle_RealisticBallistics)
            {
                ManagedParameters.SetParameter(ManagedParametersEnum.AirFrictionArrow,
                     SubModule.Settings.Battle_RealisticBallistics_AirFrictionArrow);
                ManagedParameters.SetParameter(ManagedParametersEnum.AirFrictionJavelin,
                    SubModule.Settings.Battle_RealisticBallistics_AirFrictionJavelin);
                ManagedParameters.SetParameter(ManagedParametersEnum.AirFrictionAxe,
                     SubModule.Settings.Battle_RealisticBallistics_AirFrictionAxe);
                ManagedParameters.SetParameter(ManagedParametersEnum.AirFrictionKnife,
                     SubModule.Settings.Battle_RealisticBallistics_AirFrictionKnife);
            }

            if (SubModule.Settings.Battle_PowerThrust)
            {
                ManagedParameters.SetParameter(ManagedParametersEnum.NonTipThrustHitDamageMultiplier,
                    SubModule.Settings.Battle_PowerThrust_NonTipThrustHitDamageMultiplier);
                ManagedParameters.SetParameter(ManagedParametersEnum.ThrustHitWithArmDamageMultiplier,
                    SubModule.Settings.Battle_PowerThrust_ThrustHitWithArmDamageMultiplier);
                ManagedParameters.SetParameter(ManagedParametersEnum.ThrustCombatSpeedGraphZeroProgressValue, 1.0f);
            }

        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_PowerThrust || SubModule.Settings.Battle_RealisticBallistics;
        }
    }
}
