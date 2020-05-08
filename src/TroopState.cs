using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CombatModCollection
{
    public class TroopState
    {
        public bool IsHero = false;
        public int TotalCount = 1;
        public int ExpectedDeathCount = 0;
        public int CurrentDeathCount = 0;
        public int Alive { get { return TotalCount - CurrentDeathCount; } }
        public float HitPoints;
        public float AccumulatedDamage = 0;

        private List<Weapon> Weapons = new List<Weapon>(4);
        private Item Shield = null;
        private Item Horse = null;

        // cached defense points
        private float ArmorPoints = 1.0f;      // against short melee weapons
        private bool IsUsingShield = false;
        private bool IsUsingRanged = false;

        private Weapon ChosenWeapon = Weapon.Fist;
        private Weapon FavorateWeapon = null;
        private AttackComposition _preparedAttack;
        private float _cachedHitDamage = 0;
        private int _expectedHits = 0;


        public TroopState(CharacterObject troop, bool isSeige = false)
        {
            IsHero = troop.IsHero;
            HitPoints = troop.HitPoints;
            if (SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                CalculateStatesDetailedModel(troop, isSeige);
            }
            else
            {
                ArmorPoints = troop.GetPower();
            }
        }

        private void CalculateStatesDetailedModel(CharacterObject troop, bool isSiege = false, Equipment equipment = null)
        {
            if (equipment == null)
                equipment = troop.Equipment;

            float armorSum = equipment.GetArmArmorSum() + equipment.GetHeadArmorSum() + equipment.GetHumanBodyArmorSum() + equipment.GetLegArmorSum();
            float num3 = armorSum * armorSum / (5 + equipment.GetTotalWeightOfArmor(true));
            ArmorPoints = (num3 * 10f + 4000f) / 10000f;

            for (EquipmentIndex index1 = EquipmentIndex.WeaponItemBeginSlot; index1 < EquipmentIndex.NumAllWeaponSlots; ++index1)
            {
                EquipmentElement equipmentElement1 = equipment[index1];
                if (!equipmentElement1.IsEmpty)
                {
                    if (equipmentElement1.Item.PrimaryWeapon.IsShield)
                    {
                        float proficiency = equipmentElement1.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipmentElement1.Item.RelevantSkill) / 300.0f * 0.7f;
                        float strength = GetShieldStrength(equipmentElement1.Item) * proficiency;
                        Shield = new Item { Strength = strength };
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
                                weapon.Attack.Melee = strength;
                                break;
                            case WeaponClass.OneHandedPolearm:
                            case WeaponClass.LowGripPolearm:
                                weapon.IsTwohanded = false;
                                weapon.Attack.Polearm = strength;
                                break;
                            case WeaponClass.TwoHandedPolearm:
                                weapon.IsTwohanded = false;
                                weapon.Attack.Polearm = strength;
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
                            weapon.Attack.Missile = strength;
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
                                    HasLimitedAmmo = isSiege ? false : true,
                                    RemainingAmmo = numAmmo
                                };
                                weapon.Attack.Missile = strength + ammoStrength;
                                Weapons.Add(weapon);
                                FavorateWeapon = weapon;
                            }
                        }
                    }
                }
            }

            if (!isSiege && equipment.Horse.Item != null)
            {
                float proficiency = equipment.Horse.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipment.Horse.Item.RelevantSkill) / 300.0f * 0.7f;
                float Strength = GetHorseStrength(equipment.Horse.Item, equipment[EquipmentIndex.HorseHarness].Item) * proficiency;
                Horse = new Item { Strength = Strength };
            }

            //InformationManager.DisplayMessage(new InformationMessage(troop.Name.ToString() +
            //    " " + MeleePoints + " " + RangedPoints + " " + ArmorPoints + " " + ShieldPoints + " " + RangedAmmo));
        }

        private float GetMeleeWeaponStrength(ItemObject item)
        {
            return (int)item.Tier * 0.24f + 0.8f;
        }

        private float GetRangedWeaponStrength(ItemObject item)
        {
            return ((int)item.Tier * 0.24f + 0.8f) * 0.9f;
        }

        private float GetRangedAmmoStrength(ItemObject item)
        {
            return ((int)item.Tier * 0.24f + 0.8f) * 0.1f;
        }

        private float GetThrownWeaponStrength(ItemObject item)
        {
            WeaponComponentData nweapon = item.WeaponComponent.PrimaryWeapon;
            if (nweapon.WeaponClass == WeaponClass.Stone)
            {
                return 0.6f;
            }
            return (int)item.Tier * 0.24f + 0.8f;
        }

        private float GetShieldStrength(ItemObject item)
        {
            return (int)item.Tier * 0.24f + 0.8f;
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

        public AttackComposition GetWeaponAttack(Weapon weapon, MapEventState mapEventState)
        {
            if (weapon.Range + mapEventState.StageRounds < mapEventState.BattleScale)
            {
                return new AttackComposition();
            }

            var attack = weapon.Attack;
            if (Horse != null)
            {
                attack.Melee *= (1 + Horse.Strength * 0.5f);
                attack.Polearm *= (1 + Horse.Strength * 1.0f);
            }
            return attack;
        }

        public void PrepareWeapon(MapEventState mapEventState)
        {
            if (!SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                return;
            }

            if (FavorateWeapon != null && FavorateWeapon.IsUsable)
            {
                var attack = GetWeaponAttack(FavorateWeapon, mapEventState);
                if (attack.Sum() > 0)
                {
                    _preparedAttack = attack;
                    ChosenWeapon = FavorateWeapon;
                }
            }
            else
            {
                ChosenWeapon = Weapon.Fist;
                var attack = GetWeaponAttack(ChosenWeapon, mapEventState);
                float highest = attack.Sum();
                _preparedAttack = attack;

                foreach (var weapon in Weapons)
                {
                    if (weapon.IsUsable)
                    {
                        attack = GetWeaponAttack(weapon, mapEventState);
                        if (attack.Sum() > highest)
                        {
                            _preparedAttack = attack;
                            highest = attack.Sum();
                            ChosenWeapon = weapon;
                        }
                    }
                }
            }

            if (!ChosenWeapon.IsTwohanded && Shield != null)
            {
                IsUsingShield = true;
            }
            else
            {
                IsUsingShield = false;
            }

            IsUsingRanged = ChosenWeapon.IsRanged;
        }

        public AttackComposition DoSingleAttack()
        {
            if (!SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                return new AttackComposition { Melee = ArmorPoints };
            }

            if (!ChosenWeapon.IsUsable)
            {
                return new AttackComposition();
            }
            if (ChosenWeapon.HasLimitedAmmo)
            {
                ChosenWeapon.RemainingAmmo -= 1;
            }
            return _preparedAttack;
        }

        public AttackComposition DoTotalAttack()
        {
            return DoSingleAttack() * Alive;
        }

        public bool TakeHit(AttackComposition attack, out float damage)
        {
            if (_expectedHits > 0)
            {
                damage = _cachedHitDamage;
                _expectedHits -= 1;
                if (ExpectedDeathCount > CurrentDeathCount)
                {
                    CurrentDeathCount += 1;
                    return true;
                }
            }

            if (SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                float meleeDefense = ArmorPoints;
                float missileDefense = ArmorPoints;
                float polearmDefense = ArmorPoints;
                if (IsUsingShield && Shield != null)
                {
                    meleeDefense += Shield.Strength;
                    missileDefense += 3 * Shield.Strength;
                    polearmDefense += Shield.Strength;
                }
                if (Horse != null)
                {
                    meleeDefense *= (1 + Horse.Strength);
                    if (IsUsingRanged)
                    {
                        meleeDefense *= 1.2f;
                    }
                }

                damage = attack.Melee / meleeDefense + attack.Missile / missileDefense + attack.Polearm / polearmDefense;
            }
            else
            {
                damage = attack.Melee / ArmorPoints;
            }
            _cachedHitDamage = damage;

            if (IsHero)
            {
                // Uses the vanilla hero health system
                HitPoints -= damage;
                if (HitPoints <= 20)
                {
                    CurrentDeathCount = 1;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Apply the damage to all alive members at once, and ignore the next Alive - 1 attacks
            AccumulatedDamage += damage * Alive;
            _expectedHits = Alive - 1;
            CalculateExpectedDeathCounts(damage);
            if (ExpectedDeathCount > CurrentDeathCount)
            {
                CurrentDeathCount += 1;
                return true;
            }

            return false;
        }

        private void CalculateExpectedDeathCounts(float damage)
        {
            float hitCountsToKill = HitPoints / damage;
            float ratio = AccumulatedDamage / TotalCount / HitPoints;
            ExpectedDeathCount = (int)Math.Round(Math.Pow(ratio, Math.Pow(hitCountsToKill, 0.7)) * TotalCount);
        }
    }

    public class Weapon
    {
        public static Weapon Fist;
        static Weapon()
        {
            Weapon.Fist = new Weapon();
            Weapon.Fist.Attack.Melee = 0.1f;
        }

        public AttackComposition Attack;
        public int Range = 0;
        public bool IsTwohanded = false;
        public bool HasLimitedAmmo = false;
        public int RemainingAmmo = 0;

        public bool IsUsable
        {
            get
            {
                if (HasLimitedAmmo && RemainingAmmo <= 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsRanged
        {
            get
            {
                return Range > 0;
            }
        }
    }

    public class Item
    {
        public float Strength;
        public float Health;
    }

    public struct AttackComposition
    {
        public float Melee;
        public float Missile;
        public float Polearm;

        public float Sum()
        {
            return Melee + Missile + Polearm;
        }

        public static AttackComposition operator *(AttackComposition a, float b)
        {
            return new AttackComposition
            {
                Melee = a.Melee * b,
                Missile = a.Missile * b,
                Polearm = a.Polearm * b,
            };
        }

        public static AttackComposition operator /(AttackComposition a, float b)
        {
            return new AttackComposition
            {
                Melee = a.Melee / b,
                Missile = a.Missile / b,
                Polearm = a.Polearm / b,
            };
        }

        public static AttackComposition operator +(AttackComposition a, AttackComposition b)
        {
            return new AttackComposition
            {
                Melee = a.Melee + b.Melee,
                Missile = a.Missile + b.Missile,
                Polearm = a.Polearm + b.Polearm,
            };
        }

        public bool Equals(AttackComposition b)
        {
            return Melee == b.Melee && Missile == b.Missile && Polearm == b.Polearm;
        }
    }
}