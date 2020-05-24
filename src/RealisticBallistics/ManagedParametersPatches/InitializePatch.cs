using HarmonyLib;
using TaleWorlds.Core;

namespace CombatModCollection.RealisticBallistics.ManagedParametersPatches
{
    [HarmonyPatch(typeof(ManagedParameters), "Initialize")]
    public class InitializePatch
    {
        public static void Postfix()
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

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_RealisticBallistics;
        }
    }
}
