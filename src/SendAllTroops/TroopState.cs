using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CombatModCollection.SendAllTroops
{
    public class TroopState
    {
        private readonly CharacterObject troop;
        private readonly PartyState partyState;
        private readonly bool IsHero;
        private readonly int TotalCount;
        private readonly float MaxHitPoints;

        private readonly List<Weapon> Weapons = new List<Weapon>(4);
        private readonly Item Shield = null;
        private readonly Item Horse = null;
        private readonly float Atheletics;
        private readonly float ArmorPoints;
        public readonly float Strength;

        private float AccumulatedDamage = 0;
        private int ExpectedDeathCount = 0;
        private int CurrentDeathCount = 0;
        private int Alive
        {
            get
            {
                if (IsHero)
                {
                    return troop.HeroObject.IsWounded ? 0 : 1;
                }
                else
                {
                    return TotalCount - CurrentDeathCount;
                }
            }
        }
        private bool IsUsingShield = false;
        private bool IsUsingRanged = false;
        private bool IsMounted = false;
        private Weapon ChosenWeapon = Weapon.Fist;

        private int _expectedHits = 0;

        public TroopState(PartyState _partyState, CharacterObject _troop, int count = 1)
        {
            troop = _troop;
            partyState = _partyState;
            IsHero = troop.IsHero;
            TotalCount = count;
            MaxHitPoints = troop.MaxHitPoints();
            if (Settings.Instance.Battle_SendAllTroops_DetailedCombatModel)
            {
                var template = troop.IsHero ? TroopTemplate.GetRefreshedTemplate(troop) : TroopTemplate.GetTroopTemplate(troop);
                foreach (var weapon in template.Weapons)
                {
                    Weapons.Add(weapon.Clone());
                }
                if (template.Shield != null)
                {
                    Shield = template.Shield.Clone();
                }
                if (template.Horse != null && !partyState.mapEventState.IsSiege)
                {
                    Horse = template.Horse.Clone();
                    IsMounted = true;
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

        public PartyAttackComposition GetWeaponAttack(Weapon weapon)
        {
            int bonusRange = partyState.mapEventState.IsSiege && !partyState.IsAttacker ? 1 : 0;
            if (weapon.Range + bonusRange + partyState.mapEventState.StageRounds < partyState.mapEventState.BattleScale)
            {
                return new PartyAttackComposition();
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
                        attack.Missile *= 1 - partyState.mapEventState.WallLevel * 0.2f;
                        attack.Polearm *= 1 - partyState.mapEventState.MeleePenaltyForAttacker;
                    }
                    else
                    {
                        attack.Missile *= 1 + partyState.mapEventState.WallLevel * 0.2f;
                    }
                }
                else
                {
                    if (partyState.IsAttacker)
                    {
                        attack.Melee *= 1 - partyState.mapEventState.MeleePenaltyForAttacker / 2;
                        attack.Missile *= 1 - partyState.mapEventState.WallLevel * 0.2f;
                        attack.Polearm *= 1 - partyState.mapEventState.MeleePenaltyForAttacker / 2;
                    }
                    else
                    {
                        attack.Missile *= 1 + partyState.mapEventState.WallLevel * 0.2f;
                    }
                }
                return new PartyAttackComposition
                {
                    Infantry = attack,
                };
            }
            else
            {
                if (Horse != null)
                {
                    attack.Melee *= (1 + Horse.Strength * 0.5f);
                    attack.Polearm *= (1 + Horse.Strength * 1.0f);
                    return new PartyAttackComposition
                    {
                        Mounted = attack,
                    };
                }
                else
                {
                    attack.Melee *= (1 + Atheletics);
                    attack.Polearm *= (1 + Atheletics);
                    return new PartyAttackComposition
                    {
                        Infantry = attack,
                    };
                }
            }
        }

        private PartyAttackComposition PrepareWeapon()
        {
            ChosenWeapon = Weapon.Fist;
            var attack = GetWeaponAttack(ChosenWeapon);
            float highest = attack.Sum();
            var preparedAttack = attack;

            foreach (var weapon in Weapons)
            {
                if (weapon.IsUsable)
                {
                    attack = GetWeaponAttack(weapon);
                    var preference = GetWeaponPreference(weapon);
                    if (attack.Sum() * preference > highest)
                    {
                        preparedAttack = attack;
                        highest = attack.Sum() * preference;
                        ChosenWeapon = weapon;
                    }
                }
            }
            return preparedAttack;
        }

        private float GetWeaponPreference(Weapon weapon)
        {
            if (IsMounted)
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
                if (partyState.mapEventState.IsSiege && partyState.IsAttacker && !partyState.mapEventState.GateBreached)
                {
                    preference *= 1.5f;
                }
            }

            return preference;
        }

        private PartyAttackComposition MakeSingleAttack(float consumption)
        {
            if (!Settings.Instance.Battle_SendAllTroops_DetailedCombatModel)
            {
                PartyAttackComposition attack = new PartyAttackComposition();
                attack.Infantry.Melee = Strength;
                return attack;
            }

            var preparedAttack = PrepareWeapon();
            if (!ChosenWeapon.IsTwohanded && Shield != null)
            {
                IsUsingShield = true;
            }
            else
            {
                IsUsingShield = false;
            }
            IsUsingRanged = ChosenWeapon.IsRanged;
            if (ChosenWeapon.HasLimitedAmmo && !partyState.mapEventState.IsSiege)
            {
                ChosenWeapon.RemainingAmmo -= consumption;
            }

            return preparedAttack;
        }

        public PartyAttackComposition MakeTotalAttack(float consumption)
        {
            if (Alive > 0)
            {
                return MakeSingleAttack(consumption) * Alive;
            }
            else
            {
                return new PartyAttackComposition();
            }
        }

        private float CalculateRecievedDamage(PartyAttackComposition attack)
        {
            float damage;
            if (Settings.Instance.Battle_SendAllTroops_DetailedCombatModel)
            {
                float infantryMeleeDefense = ArmorPoints;
                float infantryMissileDefense = ArmorPoints;
                float infantryPolearmDefense = ArmorPoints;

                float mountedMeleeDefense = ArmorPoints;
                float mountedMissileDefense = ArmorPoints;
                float mountedPolearmDefense = ArmorPoints;
                if (IsUsingShield && Shield != null)
                {
                    infantryMeleeDefense += Shield.Strength;
                    infantryMissileDefense += 6 * Shield.Strength;
                    infantryPolearmDefense += Shield.Strength;

                    mountedMeleeDefense += Shield.Strength;
                    mountedMissileDefense += 6 * Shield.Strength;
                    mountedPolearmDefense += Shield.Strength;
                }
                if (IsMounted)
                {
                    if (IsUsingRanged)
                    {
                        infantryMeleeDefense *= (1 + 3 * Horse.Strength);
                        infantryMissileDefense *= (1 + Horse.Strength);
                        infantryPolearmDefense *= (1 + 3 * Horse.Strength);
                    }
                    else
                    {
                        infantryMeleeDefense *= (1 + Horse.Strength);
                        infantryMissileDefense *= (1 + Horse.Strength);
                    }
                    mountedMeleeDefense *= (1 + Horse.Strength);
                    mountedMissileDefense *= (1 + Horse.Strength);
                    mountedPolearmDefense *= (1 + Horse.Strength);
                }
                else
                {
                    if (IsUsingRanged)
                    {
                        infantryMeleeDefense *= (1 + 6 * Atheletics);
                        infantryPolearmDefense *= (1 + 6 * Atheletics);
                    }
                    else
                    {
                        infantryMeleeDefense *= (1 + Atheletics);
                        infantryPolearmDefense *= (1 + Atheletics);
                    }
                    mountedMeleeDefense *= (1 + Atheletics);
                    mountedPolearmDefense *= (1 + Atheletics);
                }

                damage = attack.Infantry.Melee / infantryMeleeDefense + attack.Infantry.Missile / infantryMissileDefense
                    + attack.Infantry.Polearm / infantryPolearmDefense + attack.Mounted.Melee / mountedMeleeDefense
                    + attack.Mounted.Missile / mountedMissileDefense + attack.Mounted.Polearm / mountedPolearmDefense;
            }
            else
            {
                damage = attack.Infantry.Melee / Strength;
            }

            if (Settings.Instance.Battle_SendAllTroops_RandomDamage)
            {
                damage *= MBRandom.RandomFloat * MBRandom.RandomFloat * 4f;
            }

            return damage;
        }

        public bool TakeHit(PartyAttackComposition attack, out float totalDamage)
        {
            if (IsHero)
            {
                // Uses the vanilla hero health system
                totalDamage = CalculateRecievedDamage(attack);
                troop.HeroObject.HitPoints -= (int)Math.Round(totalDamage);
                return troop.HeroObject.IsWounded;
            }
            else
            {
                if (Alive <= 0)
                {
                    totalDamage = 0;
                    return true;
                }
                if (_expectedHits > 0)
                {
                    totalDamage = 0;
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

                float singleDamage = CalculateRecievedDamage(attack);
                // Apply the damage to all alive members at once, and ignore the next Alive - 1 attacks  
                totalDamage = Math.Min(singleDamage, MaxHitPoints) * Alive;
                if (Settings.Instance.Battle_SendAllTroops_RandomDeath)
                {
                    for (int i = 0; i < Alive; i++)
                    {
                        if (MBRandom.RandomFloat * MaxHitPoints < singleDamage)
                        {
                            ExpectedDeathCount += 1;
                        }
                    }
                }
                else
                {
                    AccumulatedDamage += totalDamage;
                    ExpectedDeathCount = (int)Math.Round(AccumulatedDamage / MaxHitPoints);
                    if (ExpectedDeathCount > TotalCount)
                    {
                        ExpectedDeathCount = TotalCount;
                    }
                }
                _expectedHits = Alive - 1;

                if (ExpectedDeathCount > CurrentDeathCount)
                {
                    CurrentDeathCount += 1;
                    return true;
                }

                return false;
            }
        }

        public float GetCurrentStrength()
        {
            // float totalStrength = Math.Max((TotalCount - AccumulatedDamage / HitPoints) * Strength, 0);
            float totalStrength = Alive * Strength;
            return totalStrength;
        }
    }
}