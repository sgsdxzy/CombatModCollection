using HarmonyLib;
using Helpers;
using System.Reflection;
using TaleWorlds.Core;


namespace CombatModCollection
{
    [HarmonyPatch(typeof(CombatStatCalculator), "CalculateStrikeMagnitudeForThrust")]
    public class CalculateStrikeMagnitudeForThrustPatch
    {        
        static bool Prefix(ref float __result,
            float thrustWeaponSpeed,
            float weaponWeight,
            float extraLinearSpeed,
            bool isThrown)
        {
            extraLinearSpeed = extraLinearSpeed / 0.5f * 0.7f; // Compensate Mission.CalculateBaseBlowMagnitude
            float num = thrustWeaponSpeed + extraLinearSpeed;
            if (!isThrown)
                weaponWeight += 2.5f;
            __result = 0.125f * (0.5f * weaponWeight * num * num) + 0.125f * weaponWeight * num + 0.067f;

            return false;
        }

        static bool Prepare()
        {
            return SubModule.Settings.Battle_PowerThrust;
        }
    }
}
