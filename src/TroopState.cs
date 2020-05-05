using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection
{
    public class TroopState
    {
        private float Hitpoints;

        // cached attack points
        private float MeleePoints = 0;
        private float MissilePoints = 0;
        private float PolearmPoints = 0;

        // cached defense points
        private float ArmorPoints = 1;
        private float ShieldPoints = 1;
        private float HorseArmorPoints = 1;

        private int RangedAmmo = 20;

        public TroopState(CharacterObject troop)
        {
            Hitpoints = troop.HitPoints;
            if (!SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                MeleePoints = troop.GetPower();
                ArmorPoints = troop.GetPower();
            }           
        }

        public AttackComposition GetAttackPoints(int StageRounds)
        {
            AttackComposition attackPoints = new AttackComposition();
            if (MissilePoints > 0 && StageRounds < RangedAmmo)
            {
                attackPoints.Missile = MissilePoints;
            } else
            {
                attackPoints.Melee = MeleePoints;
                attackPoints.Polearm = PolearmPoints;
            }

            return attackPoints;
        }

        public float GetStrength(int StageRounds)
        {
            AttackComposition attackPoints = GetAttackPoints(StageRounds);
            float totalAttack = attackPoints.Melee + attackPoints.Missile + attackPoints.Polearm;
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
        public float Polearm = 0;


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
    }
}