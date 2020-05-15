namespace CombatModCollection.SendAllTroops
{
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
}