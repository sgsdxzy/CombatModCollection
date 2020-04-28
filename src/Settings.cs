using Newtonsoft.Json;

namespace CombatModCollection
{
    public class Settings
    {
        public bool Battle_SurviveByArmor { get; set; }
        public float Battle_SurviveByArmor_ArmorThreshold { get; set; }

        public bool Battle_GoodSoildersNeverDie { get; set; }
        public bool Battle_GoodSoildersNeverDie_OnlyApplyToPlayerParty { get; set; }
        public float Battle_GoodSoildersNeverDie_MinimumLevel { get; set; }

        public bool Battle_SendAllTroops { get; set; }
        public float Battle_SendAllTroops_CombatSpeed { get; set; }

        public bool Battle_WarStomp { get; set; }
        public bool Battle_WarStomp_UnstoppableCharge { get; set; }
        public float Battle_WarStomp_WarStompDamageMultiplier { get; set; }

        public bool Battle_PowerThrust { get; set; }


        public bool Strategy_ModifyRespawnParty { get; set; }
        public float Strategy_ModifyRespawnParty_AILordPartySizeOnRespawn { get; set; }
        public float Strategy_ModifyRespawnParty_PlayerPartySizeOnRespawn { get; set; }

        public bool Strategy_LearnToQuit { get; set; }
    }
}
