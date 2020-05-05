using TaleWorlds.CampaignSystem;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace CombatModCollection
{
    public class TroopState
    {
        private float Hitpoints;

        // cached attack points
        private float MeleePoints = 0;
        private float RangedPoints = 0;
        // private float PolearmPoints = 0;

        // cached defense points
        private float ArmorPoints = 1;      // against short melee weapons
        private float ShieldPoints = 1;     // against missles
        // private float HorseArmorPoints = 1; // against polearms

        private int RangedAmmo = 0;

        public TroopState(CharacterObject troop)
        {
            Hitpoints = troop.HitPoints;
            if (!SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                MeleePoints = troop.GetPower();
                ArmorPoints = troop.GetPower();
            }
            else
            {
                CalculateStatesNewModel(troop);
            }
        }


        private void CalculateStatesNewModel(CharacterObject troop, Equipment equipment = null)
        {
            if (equipment == null)
                equipment = troop.Equipment;

            float bestMelee = 0.1f;
            float bestRanged = 0.0f;
            float bestShield = 0.0f;
            int numAmmo = 0;

            float armorSum = equipment.GetArmArmorSum() + equipment.GetHeadArmorSum() + equipment.GetHumanBodyArmorSum() + equipment.GetLegArmorSum();
            float num3 = armorSum * armorSum / (5 + equipment.GetTotalWeightOfArmor(true));
            float armorPoints = (num3 * 10f + 4000f) / 10000f;

            for (EquipmentIndex index1 = EquipmentIndex.WeaponItemBeginSlot; index1 < EquipmentIndex.NumAllWeaponSlots; ++index1)
            {
                EquipmentElement equipmentElement1 = equipment[index1];
                if (!equipmentElement1.IsEmpty)
                {
                    if (equipmentElement1.Item.PrimaryWeapon.IsShield)
                    {
                        float proficiency = equipmentElement1.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipmentElement1.Item.RelevantSkill) / 300.0f * 0.7f;
                        float strength = GetShieldStrength(equipmentElement1.Item) * proficiency;
                        bestShield = Math.Max(bestShield, strength);
                    }
                    else if (equipmentElement1.Item.PrimaryWeapon.IsMeleeWeapon)
                    {
                        float proficiency = equipmentElement1.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipmentElement1.Item.RelevantSkill) / 300.0f * 0.7f;
                        float strength = GetMeleeWeaponStrength(equipmentElement1.Item) * proficiency;
                        bestMelee = Math.Max(bestMelee, strength);
                    }
                    else if (equipmentElement1.Item.PrimaryWeapon.IsRangedWeapon)
                    {
                        if (equipmentElement1.Item.Type == ItemObject.ItemTypeEnum.Thrown)
                        {
                            float proficiency = equipmentElement1.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipmentElement1.Item.RelevantSkill) / 300.0f * 0.7f;
                            float strength = GetThrownWeaponStrength(equipmentElement1.Item) * proficiency;
                            if (strength > bestRanged)
                            {
                                numAmmo = equipmentElement1.Item.PrimaryWeapon.MaxDataValue;
                                bestRanged = strength;
                            }
                        }
                        else
                        {
                            for (EquipmentIndex index2 = EquipmentIndex.WeaponItemBeginSlot; index2 < EquipmentIndex.NumAllWeaponSlots; ++index2)
                            {
                                EquipmentElement equipmentElement2 = equipment[index2];
                                if (index1 != index2 && !equipmentElement2.IsEmpty
                                    && equipmentElement2.Item.PrimaryWeapon.IsAmmo)
                                {
                                    float proficiency = equipmentElement1.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipmentElement1.Item.RelevantSkill) / 300.0f * 0.7f;
                                    float strength = GetRangedWeaponStrength(equipmentElement1.Item, equipmentElement2.Item) * proficiency;
                                    if (strength > bestRanged)
                                    {
                                        numAmmo = equipmentElement2.Item.PrimaryWeapon.MaxDataValue;
                                        bestRanged = strength;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            float horseStrengh = 0f;
            if (equipment.Horse.Item != null)
            {
                float proficiency = equipment.Horse.Item.RelevantSkill == null ? 1f : 0.3f + troop.GetSkillValue(equipment.Horse.Item.RelevantSkill) / 300.0f * 0.7f;
                horseStrengh = proficiency * GetHorseStrength(equipment.Horse.Item, equipment[EquipmentIndex.HorseHarness].Item);
            }          

            MeleePoints = bestMelee * (1 + horseStrengh);
            RangedPoints = bestRanged;

            ArmorPoints = armorPoints * (1 + horseStrengh);
            ShieldPoints = (armorPoints + bestShield) * (1 + horseStrengh);

            if (numAmmo > 0)
            {
                RangedAmmo = Math.Max(numAmmo / 2, 1);
            }        

            InformationManager.DisplayMessage(new InformationMessage(troop.Name.ToString() +
                " " + MeleePoints + " " + RangedPoints + " " + ArmorPoints + " " + ShieldPoints + " " + RangedAmmo));
        }

        private float GetMeleeWeaponStrength(ItemObject item)
        {
            if (SubModule.Settings.Battle_SendAllTroops_SimplifiedModel)
            {
                return (int)item.Tier * 0.24f + 0.8f;
            }
            WeaponComponentData weapon = item.WeaponComponent.PrimaryWeapon;
            WeaponClass weaponClass = weapon.WeaponClass;
            float constMultiplier = 3.5e-6f;
            float classMultiplier;
            switch (weaponClass)
            {
                case WeaponClass.TwoHandedAxe:
                case WeaponClass.TwoHandedMace:
                    classMultiplier = 1.2f;
                    break;
                default:
                    classMultiplier = 1.0f;
                    break;
            }
            float thrustDPS = weapon.ThrustDamage * weapon.ThrustSpeed;
            float swingDPS = weapon.SwingDamage * weapon.SwingSpeed;
            float DPS = Math.Max(thrustDPS, swingDPS);
            double strength = constMultiplier * classMultiplier * DPS * Math.Pow(weapon.WeaponLength, 0.7)
                * Math.Pow(weapon.Handling, 0.2) * Math.Pow(item.Weight, 0.2);

            //InformationManager.DisplayMessage(new InformationMessage(item.Name.ToString() +
            //   " Melee " + strength));
            return (float)strength;
        }

        private float GetRangedWeaponStrength(ItemObject item, ItemObject ammoItem)
        {
            if (SubModule.Settings.Battle_SendAllTroops_SimplifiedModel)
            {
                return (int)item.Tier * 0.24f + 0.8f;
            }
            WeaponComponentData weapon = item.WeaponComponent.PrimaryWeapon;
            WeaponComponentData ammo = ammoItem.WeaponComponent.PrimaryWeapon;
            if (weapon.AmmoClass != ammo.WeaponClass)
            {
                return 0;
            }
            WeaponClass weaponClass = weapon.WeaponClass;
            float constMultiplier = 3.5e-5f;
            float classMultiplier;
            switch (weaponClass)
            {
                case WeaponClass.Crossbow:
                    classMultiplier = 0.7f;
                    break;
                default:
                    classMultiplier = 1.0f;
                    break;
            }
            float DPS = (weapon.ThrustDamage + ammo.MissileDamage) * weapon.ThrustSpeed;
            double strength = constMultiplier * classMultiplier * DPS * Math.Pow(weapon.Accuracy, 0.5);

            //InformationManager.DisplayMessage(new InformationMessage(item.Name.ToString() + "  " +
            //    ammoItem.Name.ToString() + " Ranged " + strength));
            return (float)strength;
        }

        private float GetThrownWeaponStrength(ItemObject item)
        {
            if (SubModule.Settings.Battle_SendAllTroops_SimplifiedModel)
            {
                WeaponComponentData nweapon = item.WeaponComponent.PrimaryWeapon;
                if (nweapon.WeaponClass == WeaponClass.Stone)
                {
                    return 0.4f;
                }
                return (int)item.Tier * 0.24f + 0.8f;
            }
            WeaponComponentData weapon = item.WeaponComponent.PrimaryWeapon;
            float constMultiplier = 1.5e-5f;

            float DPS = weapon.ThrustDamage * weapon.ThrustSpeed;
            double strength = constMultiplier * DPS * Math.Pow(weapon.Accuracy, 0.5) / 70000;

            //InformationManager.DisplayMessage(new InformationMessage(item.Name.ToString() +
            //    " Thrown " + strength));
            return (float)strength;
        }

        private float GetShieldStrength(ItemObject item)
        {
            if (SubModule.Settings.Battle_SendAllTroops_SimplifiedModel)
            {
                return (int)item.Tier * 0.24f + 0.8f;
            }
            WeaponComponentData weapon = item.WeaponComponent.PrimaryWeapon;
            float constMultiplier = 0.001f;
            double strength = constMultiplier * Math.Pow(weapon.MaxDataValue, 0.5) * 
                Math.Pow(weapon.BodyArmor, 0.5) * Math.Pow(weapon.ThrustSpeed, 0.5);

            //InformationManager.DisplayMessage(new InformationMessage(item.Name.ToString() +
            //    " Shield " + strength));
            return (float)strength;
        }

        private float GetHorseStrength(ItemObject itemHorse, ItemObject itemHarness)
        {
            if (SubModule.Settings.Battle_SendAllTroops_SimplifiedModel)
            {
                return (int)itemHorse.Tier * 0.1f;
            }

            float harnessStrength = 0.7f;
            if (itemHarness != null)
            {
                harnessStrength = (float)Math.Pow(itemHarness.ArmorComponent.BodyArmor, 0.2);
            }

            HorseComponent horse = itemHorse.HorseComponent;
            float constMultiplier = 0.001f;
            double strength = constMultiplier * harnessStrength * Math.Pow(horse.Speed, 0.5) * Math.Pow(horse.HitPoints, 0.5)
                    * Math.Pow(horse.Maneuver, 0.2) * Math.Pow(horse.ChargeDamage, 0.1);      

            //InformationManager.DisplayMessage(new InformationMessage(itemHorse.Name.ToString() +
            //    " Horse " + strength));
            return (float)strength;
        }


        public AttackComposition GetAttackPoints(int StageRounds)
        {
            AttackComposition attackPoints = new AttackComposition();
            if (RangedPoints > 0 && StageRounds < RangedAmmo)
            {
                attackPoints.Missile = RangedPoints;
            }
            else
            {
                if (StageRounds >= 1)
                {
                    attackPoints.Melee = MeleePoints;
                }               
            }

            return attackPoints;
        }

        public float GetStrength(int StageRounds)
        {
            AttackComposition attackPoints = GetAttackPoints(StageRounds);
            float totalAttack = Math.Max(attackPoints.Melee, attackPoints.Missile);
            float totalDefense = ArmorPoints;

            return totalAttack * totalDefense * Hitpoints;
        }

        public bool OnHit(AttackComposition attack, out float damage)
        {
            damage = attack.Melee / ArmorPoints + attack.Missile / ShieldPoints;
            Hitpoints -= damage;
            return Hitpoints <= 0;
        }
    }

    public class AttackComposition
    {
        public float Melee = 0;
        public float Missile = 0;
        //public float Polearm = 0;


        public static AttackComposition operator *(AttackComposition a, float b)
        {
            return new AttackComposition
            {
                Melee = a.Melee * b,
                Missile = a.Missile * b,
                //Polearm = a.Polearm * b,
            };
        }

        public static AttackComposition operator /(AttackComposition a, float b)
        {
            return new AttackComposition
            {
                Melee = a.Melee / b,
                Missile = a.Missile / b,
                //Polearm = a.Polearm / b,
            };
        }

        public static AttackComposition operator +(AttackComposition a, AttackComposition b)
        {
            return new AttackComposition
            {
                Melee = a.Melee + b.Melee,
                Missile = a.Missile + b.Missile,
                //Polearm = a.Polearm + b.Polearm,
            };
        }
    }
}