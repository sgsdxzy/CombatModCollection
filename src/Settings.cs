namespace CombatModCollection
{
    public class Settings
    {
        public bool Battle_SurviveByArmor { get; set; } = false;
        public bool Battle_SurviveByArmor_SurviveByExcessiveDamage { get; set; } = true;
        public float Battle_SurviveByArmor_SafeExcessiveDamage { get; set; } = 10f;
        public float Battle_SurviveByArmor_LethalExcessiveDamage { get; set; } = 50f;
        public bool Battle_SurviveByArmor_ApplyMedicine { get; set; } = true;
        public bool Battle_SurviveByArmor_ApplyLevel { get; set; } = false;
        public bool Battle_SurviveByArmor_ApplyArmor { get; set; } = false;
        public float Battle_SurviveByArmor_BluntDeathRate { get; set; } = 0.0f;
        public bool Battle_SurviveByArmor_SurviveByArmorValue { get; set; } = true;
        public float Battle_SurviveByArmor_ArmorValueThreshold { get; set; } = 100f;

        public bool Battle_SendAllTroops { get; set; } = false;
        public bool Battle_SendAllTroops_AbsoluteZeroRandomness { get; set; } = false;
        public bool Battle_SendAllTroops_DetailedCombatModel { get; set; } = false;
        public float Battle_SendAllTroops_CombatSpeed { get; set; } = 1.0f;
        public float Battle_SendAllTroops_XPMultiplier { get; set; } = 1.0f;

        public bool Battle_WarStomp { get; set; } = false;
        public bool Battle_WarStomp_UnstoppableWarHorseCharge { get; set; } = true;
        public bool Battle_WarStomp_UnstoppableHorseCharge { get; set; } = false;
        public float Battle_WarStomp_DamageMultiplierToHorse { get; set; } = 1.0f;
        public float Battle_WarStomp_WarStompDamageMultiplier { get; set; } = 4.0f;

        public bool Battle_PowerThrust { get; set; } = false;
        public float Battle_PowerThrust_kE { get; set; } = 0.125f;
        public float Battle_PowerThrust_kP { get; set; } = 0.1f;
        public float Battle_PowerThrust_kC { get; set; } = 0.067f;
        public float Battle_PowerThrust_ThrustHitWithArmDamageMultiplier { get; set; } = 0.15f;
        public float Battle_PowerThrust_NonTipThrustHitDamageMultiplier { get; set; } = 0.15f;

        public bool Battle_RealisticBallistics { get; set; } = false;
        public bool Battle_RealisticBallistics_ConsistantArrowSpeed { get; set; } = false;
        public float Battle_RealisticBallistics_ArrowSpeedMultiplier { get; set; } = 1.0f;
        public float Battle_RealisticBallistics_BoltSpeedMultiplier { get; set; } = 1.0f;
        public float Battle_RealisticBallistics_ThrownSpeedMultiplier { get; set; } = 1.0f;
        public float Battle_RealisticBallistics_BowAccuracyMultiplier { get; set; } = 1.0f;
        public float Battle_RealisticBallistics_CrossbowAccuracyMultiplier { get; set; } = 1.0f;
        public float Battle_RealisticBallistics_ThrownAccuracyMultiplier { get; set; } = 1.0f;
        public float Battle_RealisticBallistics_AirFrictionJavelin { get; set; } = 0.002f;
        public float Battle_RealisticBallistics_AirFrictionArrow { get; set; } = 0.003f;
        public float Battle_RealisticBallistics_AirFrictionKnife { get; set; } = 0.007f;
        public float Battle_RealisticBallistics_AirFrictionAxe { get; set; } = 0.007f;


        public bool Strategy_ModifyRespawnParty { get; set; } = false;
        public float Strategy_ModifyRespawnParty_AILordPartySizeOnRespawn { get; set; } = 3.0f;
        public float Strategy_ModifyRespawnParty_PlayerPartySizeOnRespawn { get; set; } = 0.0f;

        public bool Strategy_LearnToQuit { get; set; } = false;
        public float Strategy_LearnToQuit_RetreatChance { get; set; } = 1.0f;
        public bool Strategy_LearnToQuit_Verbose { get; set; } = true;

        public bool Strategy_BanditMerger { get; set; } = false;
        public float Strategy_BanditMerger_MergRadius { get; set; } = 15f;
        public float Strategy_BanditMerger_MaxNumber { get; set; } = 100f;
        public float Strategy_BanditMerger_MaximumBanditParties { get; set; } = 300f;
    }
}
