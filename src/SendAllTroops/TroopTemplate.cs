using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
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

        public static TroopTemplate GetTroopTemplate(CharacterObject troop)
        {
            if (troop.IsHero)
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

            float bestShield = 0.0f;
            for (EquipmentIndex index1 = EquipmentIndex.WeaponItemBeginSlot; index1 < EquipmentIndex.NumAllWeaponSlots; ++index1)
            {
                EquipmentElement equipmentElement1 = equipment[index1];
                if (!equipmentElement1.IsEmpty)
                {
                    if (equipmentElement1.Item.PrimaryWeapon.IsShield)
                    {
                        float proficiency = equipmentElement1.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipmentElement1.Item.RelevantSkill) / 300.0f * 0.7f;
                        float strength = GetShieldStrength(equipmentElement1.Item) * proficiency;
                        if (strength > bestShield)
                        {
                            bestShield = strength;
                            Shield = new Item { Strength = strength };
                        }
                    }
                    else if (equipmentElement1.Item.PrimaryWeapon.IsMeleeWeapon)
                    {
                        float proficiency = equipmentElement1.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipmentElement1.Item.RelevantSkill) / 300.0f * 0.7f;
                        float strength = GetMeleeWeaponStrength(equipmentElement1.Item) * proficiency;
                        Weapon weapon = new Weapon
                        {
                            Range = 0,
                            HasLimitedAmmo = false
                        };
                        switch (equipmentElement1.Item.PrimaryWeapon.WeaponClass)
                        {
                            case WeaponClass.Dagger:
                            case WeaponClass.OneHandedSword:
                            case WeaponClass.OneHandedAxe:
                            case WeaponClass.Mace:
                                weapon.IsTwohanded = false;
                                weapon.Attack.Melee = strength;
                                break;
                            case WeaponClass.TwoHandedSword:
                            case WeaponClass.TwoHandedAxe:
                            case WeaponClass.TwoHandedMace:
                                weapon.IsTwohanded = true;
                                weapon.Attack.Melee = strength * 1.5f;
                                break;
                            case WeaponClass.OneHandedPolearm:
                            case WeaponClass.LowGripPolearm:
                                weapon.IsTwohanded = false;
                                weapon.Attack.Polearm = strength;
                                break;
                            case WeaponClass.TwoHandedPolearm:
                                weapon.IsTwohanded = true;
                                weapon.Attack.Polearm = strength * 1.5f;
                                break;
                        }
                        Weapons.Add(weapon);
                    }
                    else if (equipmentElement1.Item.PrimaryWeapon.IsRangedWeapon)
                    {
                        if (equipmentElement1.Item.Type == ItemObject.ItemTypeEnum.Thrown)
                        {
                            float proficiency = equipmentElement1.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipmentElement1.Item.RelevantSkill) / 300.0f * 0.7f;
                            float strength = GetThrownWeaponStrength(equipmentElement1.Item) * proficiency;
                            Weapon weapon = new Weapon
                            {
                                Range = 1,
                                IsTwohanded = false,
                                HasLimitedAmmo = true,
                                RemainingAmmo = (int)Math.Round(equipmentElement1.Item.PrimaryWeapon.MaxDataValue * AmmoMultiplier)
                            };
                            weapon.Attack.Missile = strength * 1.5f;
                            Weapons.Add(weapon);
                        }
                        else
                        {
                            float proficiency = equipmentElement1.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipmentElement1.Item.RelevantSkill) / 300.0f * 0.7f;
                            float strength = GetRangedWeaponStrength(equipmentElement1.Item) * proficiency;
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
                                weapon.Attack.Missile = (strength + ammoStrength) * 1.5f;
                                Weapons.Add(weapon);
                                // FavorateWeapon = weapon;
                            }
                        }
                    }
                }
            }

            float bestHorse = Atheletics;
            if (equipment.Horse.Item != null)
            {
                float proficiency = equipment.Horse.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipment.Horse.Item.RelevantSkill) / 300.0f * 0.7f;
                float strength = GetHorseStrength(equipment.Horse.Item, equipment[EquipmentIndex.HorseHarness].Item) * proficiency;
                Horse = new Item { Strength = strength };
                bestHorse = strength;
            }

            float bestWeapon = 0.0f;
            bool twoHanded = false;
            foreach (var weapon in Weapons)
            {
                float damage = weapon.Attack.Sum();
                if (damage > bestWeapon)
                {
                    bestWeapon = damage;
                    twoHanded = weapon.IsTwohanded;
                }
            }

            Strength = (ArmorPoints + (twoHanded ? 0 : bestShield)) * bestWeapon * (1 + bestHorse);
        }

        private float GetMeleeWeaponStrength(ItemObject item)
        {
            return (int)item.Tier * 0.2f + 0.8f;
        }

        private float GetRangedWeaponStrength(ItemObject item)
        {
            return ((int)item.Tier * 0.2f + 0.8f) * 0.9f;
        }

        private float GetRangedAmmoStrength(ItemObject item)
        {
            return ((int)item.Tier * 0.2f + 0.8f) * 0.1f;
        }

        private float GetThrownWeaponStrength(ItemObject item)
        {
            WeaponComponentData nweapon = item.WeaponComponent.PrimaryWeapon;
            if (nweapon.WeaponClass == WeaponClass.Stone)
            {
                return 0.4f;
            }
            return (int)item.Tier * 0.2f + 0.8f;
        }

        private float GetShieldStrength(ItemObject item)
        {
            return ((int)item.Tier * 0.2f + 0.8f) * 0.25f;
        }

        private float GetHorseStrength(ItemObject itemHorse, ItemObject itemHarness)
        {
            float harnessStrength = 0.7f;
            if (itemHarness != null)
            {
                harnessStrength = (float)Math.Pow(itemHarness.ArmorComponent.BodyArmor, 0.4);
            }

            return itemHorse.Tierf * harnessStrength * 0.05f;
        }
    }
}