using HarmonyLib;
using System.Reflection;
using TaleWorlds.Core;

namespace CombatModCollection
{
    public class WeaponPatcher
    {
        private static readonly PropertyInfo WeaponComponentData_Accuracy = typeof(WeaponComponentData).GetProperty(
            "Accuracy", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly PropertyInfo WeaponComponentData_MissileSpeed = typeof(WeaponComponentData).GetProperty(
            "MissileSpeed", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        public static void PatchWeapon(WeaponComponentData weaponData)
        {
            WeaponComponentData_Accuracy.SetValue(weaponData, 100);
        }
    }

    [HarmonyPatch(typeof(WeaponComponentData), "Deserialize")]
    public class DeserializePatch
    {
        public static void Postfix(ref WeaponComponentData __instance)
        {
            WeaponPatcher.PatchWeapon(__instance);
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_RealisticBallistics;
        }
    }

    [HarmonyPatch(typeof(WeaponComponentData), "Init")]
    public class InitPatch
    {
        public static void Postfix(ref WeaponComponentData __instance)
        {
            WeaponPatcher.PatchWeapon(__instance);
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_RealisticBallistics;
        }
    }
}
