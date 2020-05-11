using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CombatModCollection
{
    public class TroopState
    {
        // public readonly string Name;
        private readonly PartyState partyState;
        private readonly bool IsHero;
        private readonly int TotalCount;
        private float HitPoints;

        private readonly List<Weapon> Weapons = new List<Weapon>(4);
        private readonly Item Shield = null;
        private readonly Item Horse = null;
        private readonly float Atheletics;
        private readonly float ArmorPoints;
        public readonly float Strength;

        private float AccumulatedDamage = 0;
        private int ExpectedDeathCount = 0;
        private int CurrentDeathCount = 0;
        private int Alive { get { return TotalCount - CurrentDeathCount; } }
        private bool IsUsingShield = false;
        private bool IsUsingRanged = false;
        private Weapon ChosenWeapon = Weapon.Fist;

        private AttackComposition _preparedAttack;
        private float _cachedHitDamage = 0;
        private int _expectedHits = 0;

        public TroopState(PartyState _partyState, CharacterObject troop, int count = 1)
        {
            // Name = troop.Name.ToString();
            partyState = _partyState;
            IsHero = troop.IsHero;
            TotalCount = count;
            HitPoints = troop.HitPoints;
            if (SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                var template = TroopTemplate.GetTroopTemplate(troop);
                foreach (var weapon in template.Weapons)
                {
                    Weapons.Add(weapon.Clone());
                }
                if (template.Shield != null)
                {
                    Shield = template.Shield.Clone();
                }
                if (template.Horse != null)
                {
                    Horse = template.Horse.Clone();
                }
                Atheletics = template.Atheletics;
                ArmorPoints = template.ArmorPoints;
                Strength = template.Strength;
            }
            else
            {
                Strength = troop.GetPower();
            }
        }

        public AttackComposition GetWeaponAttack(Weapon weapon)
        {
            int bonusRange = partyState.mapEventState.IsSiege && !partyState.IsAttacker ? 1 : 0;
            if (weapon.Range + bonusRange + partyState.mapEventState.StageRounds < partyState.mapEventState.BattleScale)
            {
                return new AttackComposition();
            }

            var attack = weapon.Attack;
            if (partyState.mapEventState.IsSiege)
            {
                if (partyState.mapEventState.GateBreached)
                {
                    attack.Melee *= (1 + Atheletics);
                    attack.Polearm *= (1 + Atheletics) * 0.8f;
                }
                else
                {
                    if (partyState.IsAttacker)
                    {
                        attack.Melee *= (1 + Atheletics) * 0.7f;
                        attack.Missile *= 0.8f;
                        attack.Polearm *= (1 + Atheletics) * 0.5f;
                    }
                    else
                    {
                        attack.Melee *= (1 + Atheletics);
                        attack.Missile *= 1.2f;
                        attack.Polearm *= (1 + Atheletics) * 0.8f;
                    }
                }
            }
            else
            {
                if (Horse != null)
                {
                    attack.Melee *= (1 + Horse.Strength * 0.5f);
                    attack.Polearm *= (1 + Horse.Strength * 1.0f);
                }
                else
                {
                    attack.Melee *= (1 + Atheletics);
                    attack.Polearm *= (1 + Atheletics);
                }
            }
            return attack;
        }

        public void PrepareWeapon()
        {
            if (!SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                return;
            }

            ChosenWeapon = Weapon.Fist;
            var attack = GetWeaponAttack(ChosenWeapon);
            float highest = attack.Sum();
            _preparedAttack = attack;

            foreach (var weapon in Weapons)
            {
                if (weapon.IsUsable)
                {
                    attack = GetWeaponAttack(weapon);
                    var preference = GetWeaponPreference(weapon);
                    if (attack.Sum() * preference > highest)
                    {
                        _preparedAttack = attack;
                        highest = attack.Sum() * preference;
                        ChosenWeapon = weapon;
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

        private float GetWeaponPreference(Weapon weapon)
        {
            if (Horse != null && !partyState.mapEventState.IsSiege)
            {
                if (weapon.IsRanged)
                {
                    return 2.0f;
                }
            }
            else
            {
                if (weapon.IsRanged)
                {
                    return 1.5f;
                }
            }

            float preference = 1.0f;
            if (weapon.Attack.Polearm > 0)
            {
                preference *= 1.1f;
            }
            if (Shield != null && !weapon.IsTwohanded)
            {
                preference *= 1 + 2 * Shield.Strength / ArmorPoints;
            }

            return preference;
        }

        public AttackComposition DoSingleAttack()
        {
            if (!SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                return new AttackComposition { Melee = Strength };
            }

            if (!ChosenWeapon.IsUsable)
            {
                return new AttackComposition();
            }
            if (ChosenWeapon.HasLimitedAmmo && !partyState.mapEventState.IsSiege)
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
                else
                {
                    return false;
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
                    missileDefense += 6 * Shield.Strength;
                    polearmDefense += Shield.Strength;
                }
                if (Horse != null && !partyState.mapEventState.IsSiege)
                {
                    if (IsUsingRanged)
                    {
                        meleeDefense *= (1 + 3 * Horse.Strength);
                        missileDefense *= (1 + Horse.Strength);
                        polearmDefense *= (1 + 3 * Horse.Strength);
                    }
                    else
                    {
                        meleeDefense *= (1 + Horse.Strength);
                        missileDefense *= (1 + Horse.Strength);
                    }
                }
                else
                {
                    if (IsUsingRanged)
                    {
                        meleeDefense *= (1 + 6 * Atheletics);
                        polearmDefense *= (1 + 6 * Atheletics);
                    }
                    else
                    {
                        meleeDefense *= (1 + Atheletics);
                        polearmDefense *= (1 + Atheletics);
                    }
                }

                damage = attack.Melee / meleeDefense + attack.Missile / missileDefense + attack.Polearm / polearmDefense;
            }
            else
            {
                damage = attack.Melee / Strength;
            }

            if (!SubModule.Settings.Battle_SendAllTroops_AbsoluteZeroRandomness)
            {
                damage *= MBRandom.RandomFloat * 2f;
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
            // float hitCountsToKill = HitPoints / damage;
            // float ratio = AccumulatedDamage / TotalCount / HitPoints;
            // ExpectedDeathCount = (int)Math.Round(Math.Pow(ratio, Math.Pow(hitCountsToKill, 0.7)) * TotalCount);
            ExpectedDeathCount = (int)Math.Round(AccumulatedDamage / HitPoints);
        }

        public float GetCurrentStrength()
        {
            float totalStrength = Math.Max((TotalCount - AccumulatedDamage / HitPoints) * Strength, 0);
            return totalStrength;
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

        public Weapon Clone()
        {
            return new Weapon
            {
                Attack = this.Attack,
                Range = this.Range,
                IsTwohanded = this.IsTwohanded,
                HasLimitedAmmo = this.HasLimitedAmmo,
                RemainingAmmo = this.RemainingAmmo
            };
        }

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

        public Item Clone()
        {
            return new Item
            {
                Strength = this.Strength,
                Health = this.Health
            };
        }
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