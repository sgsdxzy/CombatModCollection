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
                attack.Melee *= (1 + Atheletics);
                attack.Polearm *= (1 + Atheletics) * 0.7f;
                if (!partyState.mapEventState.GateBreached)
                {
                    if (partyState.IsAttacker)
                    {
                        attack.Melee *= 1 - partyState.mapEventState.MeleePenaltyForAttacker;
                        attack.Missile *= 1 - partyState.mapEventState.WallLevel * 0.25f;
                        attack.Polearm *= 1 - partyState.mapEventState.MeleePenaltyForAttacker;
                    }
                    else
                    {
                        attack.Missile *= 1 + partyState.mapEventState.WallLevel * 0.25f;
                    }
                }
                else
                {
                    if (partyState.IsAttacker)
                    {
                        attack.Melee *= 1 - partyState.mapEventState.MeleePenaltyForAttacker / 2;
                        attack.Missile *= 1 - partyState.mapEventState.WallLevel * 0.25f;
                        attack.Polearm *= 1 - partyState.mapEventState.MeleePenaltyForAttacker / 2;
                    }
                    else
                    {
                        attack.Missile *= 1 + partyState.mapEventState.WallLevel * 0.25f;
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

        private AttackComposition MakeSingleAttack(float consumption)
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
                ChosenWeapon.RemainingAmmo -= consumption;
            }
            return _preparedAttack;
        }

        public AttackComposition MakeTotalAttack(float consumption)
        {
            return MakeSingleAttack(consumption) * Alive;
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

            if (SubModule.Settings.Battle_SendAllTroops_RandomDamage)
            {
                damage *= MBRandom.RandomFloat * MBRandom.RandomFloat * 4f;
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
            if (SubModule.Settings.Battle_SendAllTroops_RandomDeath)
            {
                for (int i = 0; i < Alive; i++)
                {
                    if (MBRandom.RandomFloat * HitPoints < damage)
                    {
                        ExpectedDeathCount += 1;
                    }
                }

            }
            else
            {
                AccumulatedDamage += damage * Alive;
                CalculateExpectedDeathCounts(damage);
            }
            _expectedHits = Alive - 1;

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
            // float totalStrength = Math.Max((TotalCount - AccumulatedDamage / HitPoints) * Strength, 0);
            float totalStrength = Alive * Strength;
            return totalStrength;
        }
    }
}