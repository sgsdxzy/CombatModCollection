namespace CombatModCollection.SendAllTroops
{
    public struct PartyAttackComposition
    {
        public WeaponAttackComposition Infantry;
        public WeaponAttackComposition Mounted;

        public float Sum()
        {
            return Infantry.Sum() + Mounted.Sum();
        }

        public static PartyAttackComposition operator *(PartyAttackComposition a, float b)
        {
            return new PartyAttackComposition
            {
                Infantry = a.Infantry * b,
                Mounted = a.Mounted * b,
            };
        }

        public static PartyAttackComposition operator /(PartyAttackComposition a, float b)
        {
            return new PartyAttackComposition
            {
                Infantry = a.Infantry / b,
                Mounted = a.Mounted / b,
            };
        }

        public static PartyAttackComposition operator +(PartyAttackComposition a, PartyAttackComposition b)
        {
            return new PartyAttackComposition
            {
                Infantry = a.Infantry + b.Infantry,
                Mounted = a.Mounted + b.Mounted,
            };
        }

        public bool Equals(PartyAttackComposition b)
        {
            return Infantry.Equals(b.Infantry) && Mounted.Equals(b.Mounted);
        }
    }
}