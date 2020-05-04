namespace CombatModCollection
{
    public class Settings
    {
        public bool Battle_SurviveByArmor { get; set; } = false;
        public float Battle_SurviveByArmor_ArmorThreshold { get; set; } = 80f;

        public bool Battle_GoodSoildersNeverDie { get; set; } = false;
        public bool Battle_GoodSoildersNeverDie_OnlyApplyToPlayerParty { get; set; } = false;
        public float Battle_GoodSoildersNeverDie_MinimumLevel { get; set; } = 20f;

        public bool Battle_SendAllTroops { get; set; } = false;
        public bool Battle_SendAllTroops_AbsoluteZeroRandomness { get; set; } = false;
        public bool Battle_SendAllTroops_DetailedCombatModel { get; set; } = true;
        public float Battle_SendAllTroops_CombatSpeed { get; set; } = 1.0f;
        public float Battle_SendAllTroops_XPMultiplier { get; set; } = 1.0f;

        public bool Battle_WarStomp { get; set; } = false;
        public bool Battle_WarStomp_UnstoppableCharge { get; set; } = true;
        public float Battle_WarStomp_DamageMultiplierToHorse { get; set; } = 1.0f;
        public float Battle_WarStomp_WarStompDamageMultiplier { get; set; } = 4.0f;

        public bool Battle_PowerThrust { get; set; } = false;
        public float Battle_PowerThrust_kE { get; set; } = 0.125f;
        public float Battle_PowerThrust_kP { get; set; } = 0.1f;
        public float Battle_PowerThrust_kC { get; set; } = 0.067f;


        public bool Strategy_ModifyRespawnParty { get; set; } = false;
        public float Strategy_ModifyRespawnParty_AILordPartySizeOnRespawn { get; set; } = 3.0f;
        public float Strategy_ModifyRespawnParty_PlayerPartySizeOnRespawn { get; set; } = 0.0f;

        public bool Strategy_LearnToQuit { get; set; } = false;
        public bool Strategy_LearnToQuit_Verbose { get; set; } = true;
    }
}
