using MBOptionScreen.Attributes;
using MBOptionScreen.Attributes.v2;
using MBOptionScreen.Settings;

namespace CombatModCollection
{
    public class Settings : AttributeSettings<Settings>
    {
        public override string Id { get; set; } = "Light.CombatModCollection_v1";
        public override string ModName => "Combat Mod Collection";
        public override string ModuleFolderName => "CombatModCollection";


        [SettingPropertyBool(displayName: "Enable", Order = 0, RequireRestart = true, HintText = "Enable SurviveByArmor")]
        [SettingPropertyGroup("SurviveByArmor")]
        public bool Battle_SurviveByArmor { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "BluntDeathRate", minValue: 0f, maxValue: 1f, Order = 1, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("SurviveByArmor")]
        public float Battle_SurviveByArmor_BluntDeathRate { get; set; } = 0.0f;

        [SettingPropertyBool(displayName: "SurviveByExcessiveDamage", Order = 0, RequireRestart = false, HintText = "Use excessive damage to determine survival rate. Excessive damage is the excessive part of the killing blow.")]
        [SettingPropertyGroup("SurviveByArmor/SurviveByExcessiveDamage")]
        public bool Battle_SurviveByArmor_SurviveByExcessiveDamage { get; set; } = true;

        [SettingPropertyInteger(displayName: "SafeExcessiveDamage", minValue: -50, maxValue: 50, Order = 1, RequireRestart = false, HintText = "If excessive damage is within this value, one is guaranteed to survive.")]
        [SettingPropertyGroup("SurviveByArmor/SurviveByExcessiveDamage")]
        public int Battle_SurviveByArmor_SafeExcessiveDamage { get; set; } = 0;

        [SettingPropertyInteger(displayName: "LethalExcessiveDamage", minValue: 0, maxValue: 100, Order = 2, RequireRestart = false, HintText = "If excessive damage is within this value, one is guaranteed to survive.")]
        [SettingPropertyGroup("SurviveByArmor/SurviveByExcessiveDamage")]
        public int Battle_SurviveByArmor_LethalExcessiveDamage { get; set; } = 30;

        [SettingPropertyBool(displayName: "ApplyMedicine", Order = 3, RequireRestart = false, HintText = "Use excessive damage to determine survival rate. Excessive damage is the excessive part of the killing blow.")]
        [SettingPropertyGroup("SurviveByArmor/SurviveByExcessiveDamage")]
        public bool Battle_SurviveByArmor_ApplyMedicine { get; set; } = true;

        [SettingPropertyBool(displayName: "ApplyLevel", Order = 4, RequireRestart = false, HintText = "Use excessive damage to determine survival rate. Excessive damage is the excessive part of the killing blow.")]
        [SettingPropertyGroup("SurviveByArmor/SurviveByExcessiveDamage")]
        public bool Battle_SurviveByArmor_ApplyLevel { get; set; } = false;

        [SettingPropertyBool(displayName: "ApplyArmor", Order = 5, RequireRestart = false, HintText = "Use excessive damage to determine survival rate. Excessive damage is the excessive part of the killing blow.")]
        [SettingPropertyGroup("SurviveByArmor/SurviveByExcessiveDamage")]
        public bool Battle_SurviveByArmor_ApplyArmor { get; set; } = false;

        [SettingPropertyBool(displayName: "Enable", Order = 0, RequireRestart = false, HintText = "Use excessive damage to determine survival rate. Excessive damage is the excessive part of the killing blow.")]
        [SettingPropertyGroup("SurviveByArmor/SurviveByArmorValue")]
        public bool Battle_SurviveByArmor_SurviveByArmorValue { get; set; } = true;

        [SettingPropertyInteger(displayName: "ArmorValueThreshold", minValue: 1, maxValue: 400, Order = 1, RequireRestart = false, HintText = "If excessive damage is within this value, one is guaranteed to survive.")]
        [SettingPropertyGroup("SurviveByArmor/SurviveByArmorValue")]
        public int Battle_SurviveByArmor_ArmorValueThreshold { get; set; } = 100;


        [SettingPropertyBool(displayName: "Enable", Order = 0, RequireRestart = true, HintText = "Setting explanation.")]
        [SettingPropertyGroup("SendAllTroops")]
        public bool Battle_SendAllTroops { get; set; } = false;

        [SettingPropertyBool(displayName: "RandomDamage", Order = 1, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("SendAllTroops")]
        public bool Battle_SendAllTroops_RandomDamage { get; set; } = false;

        [SettingPropertyBool(displayName: "RandomDeath", Order = 2, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("SendAllTroops")]
        public bool Battle_SendAllTroops_RandomDeath { get; set; } = false;

        [SettingPropertyBool(displayName: "DetailedCombatModel", Order = 3, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("SendAllTroops")]
        public bool Battle_SendAllTroops_DetailedCombatModel { get; set; } = true;

        [SettingPropertyFloatingInteger(displayName: "CombatSpeed", minValue: 0f, maxValue: 10f, Order = 4, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("SendAllTroops/AdvancedSettings")]
        public float Battle_SendAllTroops_CombatSpeed { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "XPMultiplier", minValue: 0f, maxValue: 10f, Order = 5, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("SendAllTroops/AdvancedSettings")]
        public float Battle_SendAllTroops_XPMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "StrengthOfNumber", minValue: 0f, maxValue: 1f, Order = 0, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("SendAllTroops/AdvancedSettings")]
        public float Battle_SendAllTroops_StrengthOfNumber { get; set; } = 0.6f;

        [SettingPropertyFloatingInteger(displayName: "SiegeStrengthOfNumber", minValue: 0f, maxValue: 1f, Order = 1, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("SendAllTroops/AdvancedSettings")]
        public float Battle_SendAllTroops_SiegeStrengthOfNumber { get; set; } = 0.2f;


        [SettingPropertyBool(displayName: "Enable", Order = 0, RequireRestart = true, HintText = "Setting explanation.")]
        [SettingPropertyGroup("WarStomp")]
        public bool Battle_WarStomp { get; set; } = false;

        [SettingPropertyBool(displayName: "UnstoppableWarHorseCharge", Order = 1, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("WarStomp")]
        public bool Battle_WarStomp_UnstoppableWarHorseCharge { get; set; } = true;

        [SettingPropertyBool(displayName: "UnstoppableHorseCharge", Order = 2, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("WarStomp")]
        public bool Battle_WarStomp_UnstoppableHorseCharge { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "DamageMultiplierToHorse", minValue: 0f, maxValue: 10f, Order = 3, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("WarStomp")]
        public float Battle_WarStomp_DamageMultiplierToHorse { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "WarStompDamageMultiplier", minValue: 0f, maxValue: 10f, Order = 4, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("WarStomp")]
        public float Battle_WarStomp_WarStompDamageMultiplier { get; set; } = 4.0f;


        [SettingPropertyBool(displayName: "Enable", Order = 0, RequireRestart = true, HintText = "Setting explanation.")]
        [SettingPropertyGroup("PowerThrust")]
        public bool Battle_PowerThrust { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "kE", minValue: 0f, maxValue: 0.2f, valueFormat: "0.000", Order = 1, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("PowerThrust")]
        public float Battle_PowerThrust_kE { get; set; } = 0.125f;

        [SettingPropertyFloatingInteger(displayName: "kP", minValue: 0f, maxValue: 0.2f, valueFormat: "0.000", Order = 2, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("PowerThrust")]
        public float Battle_PowerThrust_kP { get; set; } = 0.1f;

        [SettingPropertyFloatingInteger(displayName: "kC", minValue: 0f, maxValue: 0.2f, valueFormat: "0.000", Order = 3, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("PowerThrust")]
        public float Battle_PowerThrust_kC { get; set; } = 0.067f;

        [SettingPropertyFloatingInteger(displayName: "ThrustHitWithArmDamageMultiplier", minValue: 0f, maxValue: 1f, Order = 4, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("PowerThrust")]
        public float Battle_PowerThrust_ThrustHitWithArmDamageMultiplier { get; set; } = 0.15f;

        [SettingPropertyFloatingInteger(displayName: "NonTipThrustHitDamageMultiplier", minValue: 0f, maxValue: 1f, Order = 5, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("PowerThrust")]
        public float Battle_PowerThrust_NonTipThrustHitDamageMultiplier { get; set; } = 0.15f;


        [SettingPropertyBool(displayName: "Enable", Order = 0, RequireRestart = true, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics")]
        public bool Battle_RealisticBallistics { get; set; } = false;

        [SettingPropertyBool(displayName: "ConsistantArrowSpeed", Order = 1, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics")]
        public bool Battle_RealisticBallistics_ConsistantArrowSpeed { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "ArrowSpeedMultiplier", minValue: 0.1f, maxValue: 2f, Order = 0, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/Bows")]
        public float Battle_RealisticBallistics_ArrowSpeedMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "BowAccuracyMultiplier", minValue: 0.1f, maxValue: 2f, Order = 1, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/Bows")]
        public float Battle_RealisticBallistics_BowAccuracyMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "BowDamageMultiplier", minValue: 0.1f, maxValue: 2f, Order = 2, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/Bows")]
        public float Battle_RealisticBallistics_BowDamageMultiplier { get; set; } = 1.0f;

        [SettingPropertyBool(displayName: "BowToCut", Order = 3, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/Bows")]
        public bool Battle_RealisticBallistics_BowToCut { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "BoltSpeedMultiplier", minValue: 0.1f, maxValue: 2f, Order = 0, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/Crossbows")]
        public float Battle_RealisticBallistics_BoltSpeedMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "CrossbowAccuracyMultiplier", minValue: 0.1f, maxValue: 2f, Order = 1, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/Crossbows")]
        public float Battle_RealisticBallistics_CrossbowAccuracyMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "CrossbowDamageMultiplier", minValue: 0.1f, maxValue: 2f, Order = 2, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/Crossbows")]
        public float Battle_RealisticBallistics_CrossbowDamageMultiplier { get; set; } = 1.0f;

        [SettingPropertyBool(displayName: "CrossbowToCut", Order = 3, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/Crossbows")]
        public bool Battle_RealisticBallistics_CrossbowToCut { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "ThrownSpeedMultiplier", minValue: 0.1f, maxValue: 2f, Order = 0, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/ThrownWeapons")]
        public float Battle_RealisticBallistics_ThrownSpeedMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "ThrownAccuracyMultiplier", minValue: 0.1f, maxValue: 2f, Order = 1, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/ThrownWeapons")]
        public float Battle_RealisticBallistics_ThrownAccuracyMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "ThrownDamageMultiplier", minValue: 0.1f, maxValue: 2f, Order = 2, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/ThrownWeapons")]
        public float Battle_RealisticBallistics_ThrownDamageMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "AirFrictionJavelin", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 0, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/AirFrictions")]
        public float Battle_RealisticBallistics_AirFrictionJavelin { get; set; } = 0.002f;

        [SettingPropertyFloatingInteger(displayName: "AirFrictionArrow", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 1, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/AirFrictions")]
        public float Battle_RealisticBallistics_AirFrictionArrow { get; set; } = 0.003f;

        [SettingPropertyFloatingInteger(displayName: "AirFrictionKnife", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 2, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/AirFrictions")]
        public float Battle_RealisticBallistics_AirFrictionKnife { get; set; } = 0.007f;

        [SettingPropertyFloatingInteger(displayName: "AirFrictionAxe", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 3, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("RealisticBallistics/AirFrictions")]
        public float Battle_RealisticBallistics_AirFrictionAxe { get; set; } = 0.007f;


        [SettingPropertyBool(displayName: "Enable", Order = 0, RequireRestart = true, HintText = "Setting explanation.")]
        [SettingPropertyGroup("ModifyRespawnParty")]
        public bool Strategy_ModifyRespawnParty { get; set; } = false;

        [SettingPropertyInteger(displayName: "AILordPartySizeOnRespawn", minValue: 0, maxValue: 300, Order = 1, RequireRestart = false, HintText = "If excessive damage is within this value, one is guaranteed to survive.")]
        [SettingPropertyGroup("ModifyRespawnParty")]
        public int Strategy_ModifyRespawnParty_AILordPartySizeOnRespawn { get; set; } = 3;

        [SettingPropertyInteger(displayName: "PlayerPartySizeOnRespawn", minValue: 0, maxValue: 300, Order = 2, RequireRestart = false, HintText = "If excessive damage is within this value, one is guaranteed to survive.")]
        [SettingPropertyGroup("ModifyRespawnParty")]
        public int Strategy_ModifyRespawnParty_PlayerPartySizeOnRespawn { get; set; } = 0;


        [SettingPropertyBool(displayName: "Enable", Order = 0, RequireRestart = true, HintText = "Setting explanation.")]
        [SettingPropertyGroup("LearnToQuit")]
        public bool Strategy_LearnToQuit { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "RetreatChance", minValue: 0f, maxValue: 5f, Order = 0, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("LearnToQuit")]
        public float Strategy_LearnToQuit_RetreatChance { get; set; } = 1.0f;

        [SettingPropertyBool(displayName: "Verbose", Order = 2, RequireRestart = false, HintText = "Setting explanation.")]
        [SettingPropertyGroup("LearnToQuit")]
        public bool Strategy_LearnToQuit_Verbose { get; set; } = true;


        [SettingPropertyBool(displayName: "Enable", Order = 0, RequireRestart = true, HintText = "Setting explanation.")]
        [SettingPropertyGroup("BanditMerger")]
        public bool Strategy_BanditMerger { get; set; } = false;

        [SettingPropertyInteger(displayName: "MergRadius", minValue: 0, maxValue: 100, Order = 1, RequireRestart = false, HintText = "If excessive damage is within this value, one is guaranteed to survive.")]
        [SettingPropertyGroup("BanditMerger")]
        public int Strategy_BanditMerger_MergRadius { get; set; } = 15;

        [SettingPropertyInteger(displayName: "MaxNumberForMerge", minValue: 0, maxValue: 300, Order = 2, RequireRestart = false, HintText = "If excessive damage is within this value, one is guaranteed to survive.")]
        [SettingPropertyGroup("BanditMerger")]
        public int Strategy_BanditMerger_MaxNumberForMerge { get; set; } = 40;

        [SettingPropertyInteger(displayName: "MaximumLooterParties", minValue: 0, maxValue: 1000, Order = 3, RequireRestart = false, HintText = "If excessive damage is within this value, one is guaranteed to survive.")]
        [SettingPropertyGroup("BanditMerger")]
        public int Strategy_BanditMerger_MaximumLooterParties { get; set; } = 300;
    }
}
