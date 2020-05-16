namespace CombatModCollection.SendAllTroops
{
    public class Weapon
    {
        public static Weapon Fist;
        static Weapon()
        {
            Weapon.Fist = new Weapon();
            Weapon.Fist.Attack.Melee = 0.1f;
        }

        public WeaponAttackComposition Attack;
        public int Range = 0;
        public bool IsTwohanded = false;
        public bool HasLimitedAmmo = false;
        public float RemainingAmmo = 0;

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
}