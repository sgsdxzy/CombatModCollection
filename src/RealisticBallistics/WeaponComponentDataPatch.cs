using HarmonyLib;
using System;
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
            switch (weaponData.WeaponClass)
            {
                case WeaponClass.Bow:
                    float missileSpeed = weaponData.MissileSpeed;
                    if (SubModule.Settings.Battle_RealisticBallistics_ConsistantArrowSpeed)
                    {
                        missileSpeed = weaponData.MissileDamage;
                    }
                    missileSpeed *= SubModule.Settings.Battle_RealisticBallistics_ArrowSpeedMultiplier;
                    float accuracy = weaponData.Accuracy;
                    accuracy *= SubModule.Settings.Battle_RealisticBallistics_BowAccuracyMultiplier;
                    if (accuracy > 100)
                    {
                        accuracy = 100;
                    }
                    WeaponComponentData_MissileSpeed.SetValue(weaponData, (int)Math.Round(missileSpeed));
                    WeaponComponentData_Accuracy.SetValue(weaponData, (int)Math.Round(accuracy));
                    break;
                case WeaponClass.Crossbow:
                    missileSpeed = weaponData.MissileSpeed;
                    if (SubModule.Settings.Battle_RealisticBallistics_ConsistantArrowSpeed)
                    {
                        missileSpeed = weaponData.MissileDamage;
                    }
                    missileSpeed *= SubModule.Settings.Battle_RealisticBallistics_BoltSpeedMultiplier;
                    accuracy = weaponData.Accuracy;
                    accuracy *= SubModule.Settings.Battle_RealisticBallistics_CrossbowAccuracyMultiplier;
                    if (accuracy > 100)
                    {
                        accuracy = 100;
                    }
                    WeaponComponentData_MissileSpeed.SetValue(weaponData, (int)Math.Round(missileSpeed));
                    WeaponComponentData_Accuracy.SetValue(weaponData, (int)Math.Round(accuracy));
                    break;
                case WeaponClass.Javelin:
                case WeaponClass.ThrowingAxe:
                case WeaponClass.ThrowingKnife:
                    missileSpeed = weaponData.MissileSpeed;
                    missileSpeed *= SubModule.Settings.Battle_RealisticBallistics_ThrownSpeedMultiplier;
                    accuracy = weaponData.Accuracy;
                    accuracy *= SubModule.Settings.Battle_RealisticBallistics_ThrownAccuracyMultiplier;
                    if (accuracy > 100)
                    {
                        accuracy = 100;
                    }
                    WeaponComponentData_MissileSpeed.SetValue(weaponData, (int)Math.Round(missileSpeed));
                    WeaponComponentData_Accuracy.SetValue(weaponData, (int)Math.Round(accuracy));
                    break;
            }
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
