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

        public List<Weapon> Weapons = new List<Weapon>(4);
        public Item Shield = null;
        public Item Horse = null;
        public float Atheletics;
        public float ArmorPoints;
        public float Strength;

        public static TroopTemplate GetTroopTemplate(CharacterObject troop)
        {
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
            ArmorPoints = (float)(0.2 + Math.Pow(armorSum, 0.5) / 12.5);

            for (EquipmentIndex index1 = EquipmentIndex.WeaponItemBeginSlot; index1 < EquipmentIndex.NumAllWeaponSlots; ++index1)
            {
                EquipmentElement equipmentElement1 = equipment[index1];
                if (!equipmentElement1.IsEmpty)
                {
                    if (equipmentElement1.Item.PrimaryWeapon.IsShield)
                    {
                        float proficiency = equipmentElement1.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipmentElement1.Item.RelevantSkill) / 300.0f * 0.7f;
                        float strength = GetShieldStrength(equipmentElement1.Item) * proficiency;
                        Shield = new Item { Strength = strength * 0.25f };
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
                                RemainingAmmo = equipmentElement1.Item.PrimaryWeapon.MaxDataValue
                            };
                            weapon.Attack.Missile = strength * 1.5f;
                            Weapons.Add(weapon);
                        }
                        else
                        {
                            float proficiency = equipmentElement1.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipmentElement1.Item.RelevantSkill) / 300.0f * 0.7f;
                            float strength = GetRangedWeaponStrength(equipmentElement1.Item) * proficiency;
                            int numAmmo = 0;
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
                                Weapon weapon = new Weapon
                                {
                                    Range = 2,
                                    IsTwohanded = true,
                                    HasLimitedAmmo = true,
                                    RemainingAmmo = numAmmo
                                };
                                weapon.Attack.Missile = (strength + ammoStrength) * 1.5f;
                                Weapons.Add(weapon);
                                // FavorateWeapon = weapon;
                            }
                        }
                    }
                }

                Strength = troop.GetPower();
            }

            if (equipment.Horse.Item != null)
            {
                float proficiency = equipment.Horse.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipment.Horse.Item.RelevantSkill) / 300.0f * 0.7f;
                float Strength = GetHorseStrength(equipment.Horse.Item, equipment[EquipmentIndex.HorseHarness].Item) * proficiency;
                Horse = new Item { Strength = Strength };
            }
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
            return (int)item.Tier * 0.2f + 0.8f;
        }

        private float GetHorseStrength(ItemObject itemHorse, ItemObject itemHarness)
        {
            float harnessStrength = 0.7f;
            if (itemHarness != null)
            {
                harnessStrength = (float)Math.Pow(itemHarness.ArmorComponent.BodyArmor, 0.2);
            }

            return (int)itemHorse.Tier * harnessStrength * 0.05f;
        }
    }
}