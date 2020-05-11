using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection
{
    public class TroopTemplate
    {
        private static readonly ConcurrentDictionary<MBGUID, TroopTemplate> CachedTemplates = new ConcurrentDictionary<MBGUID, TroopTemplate>();
        private static readonly float AmmoMultiplier = 1.0f;

        public readonly List<Weapon> Weapons = new List<Weapon>(4);
        public readonly Item Shield = null;
        public readonly Item Horse = null;
        public readonly float Atheletics;
        public readonly float ArmorPoints;
        public readonly float Strength;

        // private static System.IO.StreamWriter templateFile = new System.IO.StreamWriter(@"D:\TroopTemplates.txt");

        public static TroopTemplate GetTroopTemplate(CharacterObject troop)
        {
            if (troop.IsHero && troop.HeroObject.Clan == Clan.PlayerClan)
            {
                return new TroopTemplate(troop);
            }

            if (!CachedTemplates.TryGetValue(troop.Id, out TroopTemplate template))
            {
                CachedTemplates[troop.Id] = new TroopTemplate(troop);
                return CachedTemplates[troop.Id];
            }
            else
            {
                return template;
            }
        }

        private TroopTemplate(CharacterObject troop)
        {
            Equipment equipment = troop.Equipment;
            float armorSum = equipment.GetArmArmorSum() + equipment.GetHeadArmorSum() + equipment.GetHumanBodyArmorSum() + equipment.GetLegArmorSum();
            float totalWeight = equipment.GetTotalWeightOfArmor(true) + equipment.GetTotalWeightOfWeapons();
            Atheletics = troop.GetSkillValue(DefaultSkills.Athletics) / (totalWeight + 3f) * 0.01f;
            ArmorPoints = (float)(0.3 + Math.Pow(armorSum, 0.75) / 40.0);

            //templateFile.WriteLine(troop.Name);
            //templateFile.WriteLine("Atheletics: " + Atheletics);
            //templateFile.WriteLine("ArmorPoints: " + ArmorPoints);

            float bestShield = 0;
            for (EquipmentIndex index1 = EquipmentIndex.WeaponItemBeginSlot; index1 < EquipmentIndex.NumAllWeaponSlots; ++index1)
            {
                EquipmentElement equipmentElement1 = equipment[index1];
                if (!equipmentElement1.IsEmpty)
                {
                    if (equipmentElement1.Item.PrimaryWeapon.IsShield)
                    {
                        float proficiency = GetProficiency(troop, equipmentElement1.Item.RelevantSkill);
                        float strength = GetShieldStrength(equipmentElement1.Item);
                        if (strength * proficiency > bestShield)
                        {
                            bestShield = strength * proficiency;
                            Shield = new Item { Strength = strength * proficiency };
                        }

                        //templateFile.WriteLine(String.Format("Shield: {0} {1:G3}*{2:G3}={3:G3}",
                        //    equipmentElement1.Item.Name, strength, proficiency, strength * proficiency));
                    }
                    else if (equipmentElement1.Item.PrimaryWeapon.IsMeleeWeapon)
                    {
                        //templateFile.WriteLine("Melee Weapon: " + equipmentElement1.Item.Name);
                        foreach (var weaponData in equipmentElement1.Item.Weapons)
                        {
                            float proficiency = GetProficiency(troop, weaponData.RelevantSkill);
                            float strength = GetMeleeWeaponStrength(weaponData);
                            //templateFile.WriteLine(String.Format("Mode: {0:G3}*{1:G3}={2:G3}",
                            //    strength, proficiency, strength * proficiency));
                            Weapon weapon = new Weapon
                            {
                                Range = 0,
                                HasLimitedAmmo = false
                            };
                            switch (weaponData.WeaponClass)
                            {
                                case WeaponClass.Dagger:
                                case WeaponClass.OneHandedSword:
                                case WeaponClass.OneHandedAxe:
                                case WeaponClass.Mace:
                                    weapon.IsTwohanded = false;
                                    weapon.Attack.Melee = strength * proficiency;
                                    break;
                                case WeaponClass.TwoHandedSword:
                                case WeaponClass.TwoHandedAxe:
                                case WeaponClass.TwoHandedMace:
                                    weapon.IsTwohanded = true;
                                    weapon.Attack.Melee = strength * proficiency;
                                    break;
                                case WeaponClass.OneHandedPolearm:
                                case WeaponClass.LowGripPolearm:
                                    weapon.IsTwohanded = false;
                                    weapon.Attack.Polearm = strength * proficiency;
                                    break;
                                case WeaponClass.TwoHandedPolearm:
                                    weapon.IsTwohanded = true;
                                    weapon.Attack.Polearm = strength * proficiency;
                                    break;
                            }
                            Weapons.Add(weapon);
                        }
                    }
                    else if (equipmentElement1.Item.PrimaryWeapon.IsRangedWeapon)
                    {
                        if (equipmentElement1.Item.Type == ItemObject.ItemTypeEnum.Thrown)
                        {
                            float proficiency = GetProficiency(troop, equipmentElement1.Item.RelevantSkill);
                            float strength = GetThrownWeaponStrength(equipmentElement1.Item.PrimaryWeapon);
                            //templateFile.WriteLine(String.Format("Thrown: {0} {1:G3}*{2:G3}={3:G3}",
                            //    equipmentElement1.Item.Name, strength, proficiency, strength * proficiency));
                            Weapon weapon = new Weapon
                            {
                                Range = 1,
                                IsTwohanded = false,
                                HasLimitedAmmo = true,
                                RemainingAmmo = (int)Math.Round(equipmentElement1.Item.PrimaryWeapon.MaxDataValue * AmmoMultiplier)
                            };
                            weapon.Attack.Missile = strength * proficiency;
                            Weapons.Add(weapon);
                        }
                        else
                        {
                            float proficiency = GetProficiency(troop, equipmentElement1.Item.RelevantSkill);
                            float strength = GetRangedWeaponStrength(equipmentElement1.Item);
                            float numAmmo = 0;
                            float ammoStrength = 0;
                            for (EquipmentIndex index2 = EquipmentIndex.WeaponItemBeginSlot; index2 < EquipmentIndex.NumAllWeaponSlots; ++index2)
                            {
                                EquipmentElement equipmentElement2 = equipment[index2];
                                if (index1 != index2 && !equipmentElement2.IsEmpty
                                    && equipmentElement1.Item.WeaponComponent.PrimaryWeapon.AmmoClass == equipmentElement2.Item.WeaponComponent.PrimaryWeapon.WeaponClass)
                                {
                                    numAmmo += equipmentElement2.Item.PrimaryWeapon.MaxDataValue;
                                    ammoStrength = GetRangedAmmoStrength(equipmentElement2.Item);
                                }
                            }
                            if (numAmmo > 0)
                            {
                                if (equipmentElement1.Item.Type == ItemObject.ItemTypeEnum.Crossbow)
                                {
                                    numAmmo *= 1.5f;
                                }
                                Weapon weapon = new Weapon
                                {
                                    Range = 3,
                                    IsTwohanded = true,
                                    HasLimitedAmmo = true,
                                    RemainingAmmo = (int)Math.Round(numAmmo * AmmoMultiplier)
                                };
                                weapon.Attack.Missile = (strength + ammoStrength) * proficiency;
                                Weapons.Add(weapon);
                                //templateFile.WriteLine(String.Format("Ranged: {0} ({1:G3}+{2:G3})*{3:G3}={4:G3}",
                                //    equipmentElement1.Item.Name, strength, ammoStrength, proficiency, (strength + ammoStrength) * proficiency));
                            }
                        }
                    }
                }
            }

            float bestMovementBonus = Atheletics;
            if (equipment.Horse.Item != null)
            {
                float proficiency = GetProficiency(troop, equipment.Horse.Item.RelevantSkill);
                float strength = GetHorseStrength(equipment.Horse.Item, equipment[EquipmentIndex.HorseHarness].Item);
                Horse = new Item { Strength = strength * proficiency };
                bestMovementBonus = strength * proficiency;

                //templateFile.WriteLine(String.Format("Horse: {0:G3}*{1:G3}={2:G3}",
                //    strength, proficiency, strength * proficiency));
            }

            float bestWeaponStrength = 0.0f;
            bool isBestWeaponTwoHanded = false;
            foreach (var weapon in Weapons)
            {
                float weaponStrength = weapon.Attack.Sum();
                if (weaponStrength > bestWeaponStrength)
                {
                    bestWeaponStrength = weaponStrength;
                    isBestWeaponTwoHanded = weapon.IsTwohanded;
                }
            }

            Strength = (ArmorPoints + (isBestWeaponTwoHanded ? 0 : bestShield)) * bestWeaponStrength * (1 + bestMovementBonus) * troop.MaxHitPoints() / 100;
            //templateFile.WriteLine("Strength: " + Strength);
            //templateFile.WriteLine();
            //templateFile.Flush();
        }

        private static float GetProficiency(CharacterObject troop, SkillObject skill)
        {
            return skill == null ? 1f : 0.6f + troop.GetSkillValue(skill) / 300.0f * 0.4f;
        }

        private static float GetFactor(DamageTypes swingDamageType)
        {
            if (swingDamageType == DamageTypes.Blunt)
                return 1.3f;
            return swingDamageType != DamageTypes.Pierce ? 1f : 1.15f;
        }

        private float GetMeleeWeaponStrength(WeaponComponentData weaponData)
        {
            float num1 = Math.Max((float)weaponData.ThrustDamage * GetFactor(weaponData.ThrustDamageType)
                * MathF.Pow((float)weaponData.ThrustSpeed * 0.01f, 1.5f),
                (float)weaponData.SwingDamage * GetFactor(weaponData.SwingDamageType)
                * MathF.Pow((float)weaponData.SwingSpeed * 0.01f, 1.5f));
            float num2 = (float)weaponData.WeaponLength * 0.01f;
            float tierf = (float)(0.06 * ((double)num1 * (1.0 + (double)num2)) - 3.5);
            tierf = MathF.Clamp(tierf, 0, 10);
            return tierf * 0.2f + 0.8f;
        }

        private float GetThrownWeaponStrength(WeaponComponentData weaponData)
        {
            if (weaponData.WeaponClass == WeaponClass.Stone)
            {
                return 0.5f;
            }
            float num1 = (float)weaponData.ThrustDamage * GetFactor(weaponData.ThrustDamageType)
                * MathF.Pow((float)weaponData.ThrustSpeed * 0.01f, 1.5f);
            float tierf = (float)(0.045 * ((double)num1 - 3.5));
            tierf = MathF.Clamp(tierf, 0, 10);
            return tierf * 0.2f + 0.8f;
        }

        private float GetRangedWeaponStrength(ItemObject item)
        {
            WeaponComponentData weaponData = item.PrimaryWeapon;
            double num1;
            switch (item.ItemType)
            {
                case ItemObject.ItemTypeEnum.Crossbow:
                    num1 = 0.7;
                    break;
                case ItemObject.ItemTypeEnum.Musket:
                    num1 = 0.5;
                    break;
                default:
                    num1 = 1.0;
                    break;
            }
            double num2 = (double)weaponData.ThrustDamage * 0.2 + (double)weaponData.ThrustSpeed * 0.02 + (double)weaponData.Accuracy * 0.02;
            float tierf = (float)(num1 * num2 - 11.0);
            tierf = MathF.Clamp(tierf, 0, 10);

            return tierf * 0.2f + 0.8f;
        }

        private float GetRangedAmmoStrength(ItemObject item)
        {
            var weaponData = item.PrimaryWeapon;
            float tier = (float)weaponData.MissileDamage;
            return (tier * 0.2f + 0.8f) * 0.1f;
        }

        private float GetShieldStrength(ItemObject item)
        {
            var weaponData = item.PrimaryWeapon;
            float tierf = (float)((weaponData.MaxDataValue + 3.0 * weaponData.BodyArmor + weaponData.ThrustSpeed) / (6.0 + weaponData.Item.Weight) * 0.13 - 3.0);
            tierf = MathF.Clamp(tierf, 0, 10);
            return (tierf * 0.2f + 0.8f) * 0.25f;
        }

        private float GetHorseStrength(ItemObject itemHorse, ItemObject itemHarness)
        {
            float harnessStrength = 0.7f;
            if (itemHarness != null)
            {
                harnessStrength = (float)Math.Pow(itemHarness.ArmorComponent.BodyArmor, 0.4);
            }

            var horseComponent = itemHorse.HorseComponent;
            float tierf = (float)((horseComponent.IsPackAnimal ? 25.0 :
                0.5 * (double)horseComponent.Maneuver + 1.5 * (double)horseComponent.Speed
                + 1 * (double)horseComponent.ChargeDamage + 0.1 * (double)horseComponent.HitPoints) / 15.0 - 8.0);
            tierf = MathF.Clamp(tierf, 1, 6);

            return tierf * harnessStrength * 0.05f;
        }
    }
}