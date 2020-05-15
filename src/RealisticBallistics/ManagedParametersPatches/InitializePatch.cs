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
                 Settings.Instance.Battle_RealisticBallistics_AirFrictionArrow);
            ManagedParameters.SetParameter(ManagedParametersEnum.AirFrictionJavelin,
                Settings.Instance.Battle_RealisticBallistics_AirFrictionJavelin);
            ManagedParameters.SetParameter(ManagedParametersEnum.AirFrictionAxe,
                 Settings.Instance.Battle_RealisticBallistics_AirFrictionAxe);
            ManagedParameters.SetParameter(ManagedParametersEnum.AirFrictionKnife,
                 Settings.Instance.Battle_RealisticBallistics_AirFrictionKnife);
        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_RealisticBallistics;
        }
    }
}
