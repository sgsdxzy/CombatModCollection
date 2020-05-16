namespace CombatModCollection.SendAllTroops
{
    public struct WeaponAttackComposition
    {
        public float Melee;
        public float Missile;
        public float Polearm;

        public float Sum()
        {
            return Melee + Missile + Polearm;
        }

        public static WeaponAttackComposition operator *(WeaponAttackComposition a, float b)
        {
            return new WeaponAttackComposition
            {
                Melee = a.Melee * b,
                Missile = a.Missile * b,
                Polearm = a.Polearm * b,
            };
        }

        public static WeaponAttackComposition operator /(WeaponAttackComposition a, float b)
        {
            return new WeaponAttackComposition
            {
                Melee = a.Melee / b,
                Missile = a.Missile / b,
                Polearm = a.Polearm / b,
            };
        }

        public static WeaponAttackComposition operator +(WeaponAttackComposition a, WeaponAttackComposition b)
        {
            return new WeaponAttackComposition
            {
                Melee = a.Melee + b.Melee,
                Missile = a.Missile + b.Missile,
                Polearm = a.Polearm + b.Polearm,
            };
        }

        public bool Equals(WeaponAttackComposition b)
        {
            return Melee == b.Melee && Missile == b.Missile && Polearm == b.Polearm;
        }
    }
}