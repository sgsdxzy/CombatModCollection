namespace CombatModCollection
{
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