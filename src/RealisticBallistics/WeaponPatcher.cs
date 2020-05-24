using System;
using System.Reflection;
using TaleWorlds.Core;

namespace CombatModCollection.RealisticBallistics
{
    public class WeaponPatcher
    {
        private static readonly PropertyInfo WeaponComponentData_Accuracy = typeof(WeaponComponentData).GetProperty(
            "Accuracy", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly PropertyInfo WeaponComponentData_MissileSpeed = typeof(WeaponComponentData).GetProperty(
            "MissileSpeed", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly PropertyInfo WeaponComponentData_ThrustDamage = typeof(WeaponComponentData).GetProperty(
            "ThrustDamage", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly PropertyInfo WeaponComponentData_ThrustDamageType = typeof(WeaponComponentData).GetProperty(
            "ThrustDamageType", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

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
                    float damage = weaponData.ThrustDamage;
                    damage *= SubModule.Settings.Battle_RealisticBallistics_BowDamageMultiplier;
                    WeaponComponentData_MissileSpeed.SetValue(weaponData, (int)Math.Round(missileSpeed));
                    WeaponComponentData_Accuracy.SetValue(weaponData, (int)Math.Round(accuracy));
                    WeaponComponentData_ThrustDamage.SetValue(weaponData, (int)Math.Round(damage));
                    if (SubModule.Settings.Battle_RealisticBallistics_BowToCut)
                    {
                        WeaponComponentData_ThrustDamageType.SetValue(weaponData, DamageTypes.Cut);
                    }
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
                    damage = weaponData.ThrustDamage;
                    damage *= SubModule.Settings.Battle_RealisticBallistics_CrossbowDamageMultiplier;
                    WeaponComponentData_MissileSpeed.SetValue(weaponData, (int)Math.Round(missileSpeed));
                    WeaponComponentData_Accuracy.SetValue(weaponData, (int)Math.Round(accuracy));
                    WeaponComponentData_ThrustDamage.SetValue(weaponData, (int)Math.Round(damage));
                    if (SubModule.Settings.Battle_RealisticBallistics_CrossbowToCut)
                    {
                        WeaponComponentData_ThrustDamageType.SetValue(weaponData, DamageTypes.Cut);
                    }
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
                    damage = weaponData.ThrustDamage;
                    damage *= SubModule.Settings.Battle_RealisticBallistics_ThrownDamageMultiplier;
                    WeaponComponentData_MissileSpeed.SetValue(weaponData, (int)Math.Round(missileSpeed));
                    WeaponComponentData_Accuracy.SetValue(weaponData, (int)Math.Round(accuracy));
                    WeaponComponentData_ThrustDamage.SetValue(weaponData, (int)Math.Round(damage));
                    break;
                case WeaponClass.Arrow:
                    if (SubModule.Settings.Battle_RealisticBallistics_BowToCut)
                    {
                        WeaponComponentData_ThrustDamageType.SetValue(weaponData, DamageTypes.Cut);
                    }
                    break;
                case WeaponClass.Bolt:
                    if (SubModule.Settings.Battle_RealisticBallistics_CrossbowToCut)
                    {
                        WeaponComponentData_ThrustDamageType.SetValue(weaponData, DamageTypes.Cut);
                    }
                    break;
            }
        }
    }
}
