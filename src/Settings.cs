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


        [SettingPropertyBool(displayName: "SurviveByArmor", Order = 0, RequireRestart = true, HintText = "Enable SurviveByArmor.")]
        [SettingPropertyGroup(groupName: "SurviveByArmor", order: 0, isMainToggle: true)]
        public bool Battle_SurviveByArmor { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "Blunt Death Rate", minValue: 0f, maxValue: 1f, Order = 1, RequireRestart = false, HintText = "The death rate of blunt weapons.")]
        [SettingPropertyGroup(groupName: "SurviveByArmor")]
        public float Battle_SurviveByArmor_BluntDeathRate { get; set; } = 0.0f;

        [SettingPropertyBool(displayName: "Survive By ExcessiveDamage", Order = 0, RequireRestart = false, HintText = "Use excessive damage to determine survival rate. Excessive damage is the excessive part of the killing blow.")]
        [SettingPropertyGroup(groupName: "SurviveByArmor/Survive By Excessive Damage", order: 2, isMainToggle: true)]
        public bool Battle_SurviveByArmor_SurviveByExcessiveDamage { get; set; } = true;

        [SettingPropertyInteger(displayName: "Safe Excessive Damage", minValue: -50, maxValue: 50, Order = 1, RequireRestart = false, HintText = "If excessive damage is within this value, one is guaranteed to survive.")]
        [SettingPropertyGroup(groupName: "SurviveByArmor/Survive By Excessive Damage")]
        public int Battle_SurviveByArmor_SafeExcessiveDamage { get; set; } = 0;

        [SettingPropertyInteger(displayName: "Lethal Excessive Damage", minValue: 0, maxValue: 100, Order = 2, RequireRestart = false, HintText = "If excessive damage is beyond this value, one is guaranteed to die unless saved by surgeon.")]
        [SettingPropertyGroup(groupName: "SurviveByArmor/Survive By Excessive Damage")]
        public int Battle_SurviveByArmor_LethalExcessiveDamage { get; set; } = 30;

        [SettingPropertyBool(displayName: "Apply Medicine Bonus", Order = 3, RequireRestart = false, HintText = "Apply survival bonus from the medicine level of party surgeon.")]
        [SettingPropertyGroup(groupName: "SurviveByArmor/Survive By Excessive Damage")]
        public bool Battle_SurviveByArmor_ApplyMedicine { get; set; } = true;

        [SettingPropertyBool(displayName: "Apply Level Bonus", Order = 4, RequireRestart = false, HintText = "Apply survival bonus from one's character level.")]
        [SettingPropertyGroup(groupName: "SurviveByArmor/Survive By Excessive Damage")]
        public bool Battle_SurviveByArmor_ApplyLevel { get; set; } = false;

        [SettingPropertyBool(displayName: "Apply Armor Bonus", Order = 5, RequireRestart = false, HintText = "Apply survival bonus from one's armor value.")]
        [SettingPropertyGroup(groupName: "SurviveByArmor/Survive By Excessive Damage")]
        public bool Battle_SurviveByArmor_ApplyArmor { get; set; } = false;

        [SettingPropertyBool(displayName: "Survive By Armor Value", Order = 0, RequireRestart = false, HintText = "Use Armor value instead of character level to determine survival chance when \"Survive By ExcessiveDamage\" is off, or in battle simulations")]
        [SettingPropertyGroup(groupName: "SurviveByArmor/Survive By Armor Value", order: 3, isMainToggle: true)]
        public bool Battle_SurviveByArmor_SurviveByArmorValue { get; set; } = true;

        [SettingPropertyInteger(displayName: "Armor Value Threshold", minValue: 1, maxValue: 400, Order = 1, RequireRestart = false, HintText = "The armor value threshold to give a medium survival chance")]
        [SettingPropertyGroup(groupName: "SurviveByArmor/Survive By Armor Value")]
        public int Battle_SurviveByArmor_ArmorValueThreshold { get; set; } = 100;


        [SettingPropertyBool(displayName: "SendAllTroops", Order = 0, RequireRestart = true, HintText = "Enable SendAllTroops.")]
        [SettingPropertyGroup(groupName: "SendAllTroops", order: 1, isMainToggle: true)]
        public bool Battle_SendAllTroops { get; set; } = false;

        [SettingPropertyBool(displayName: "Random Damage", Order = 1, RequireRestart = false, HintText = "If set to false, the damage value is fixed. If true, it will randomly fluctuate around the center points.")]
        [SettingPropertyGroup(groupName: "SendAllTroops")]
        public bool Battle_SendAllTroops_RandomDamage { get; set; } = false;

        [SettingPropertyBool(displayName: "Random Death", Order = 2, RequireRestart = false, HintText = "If set to false, the death of soldier is determined by hitpoints reach 0. If true, the vanilla instant-death-by-chance is used.")]
        [SettingPropertyGroup(groupName: "SendAllTroops")]
        public bool Battle_SendAllTroops_RandomDeath { get; set; } = false;

        [SettingPropertyBool(displayName: "Detailed Combat Model", Order = 3, RequireRestart = false, HintText = "Whether to use equipments and skill levels of a soldier to determine his strength, and simulate the progression of battles. If set to false, the vanilla Power is used.")]
        [SettingPropertyGroup(groupName: "SendAllTroops")]
        public bool Battle_SendAllTroops_DetailedCombatModel { get; set; } = true;

        [SettingPropertyFloatingInteger(displayName: "Combat Speed", minValue: 0f, maxValue: 10f, Order = 4, RequireRestart = false, HintText = "Multiply the combat speed between AIs.")]
        [SettingPropertyGroup(groupName: "SendAllTroops")]
        public float Battle_SendAllTroops_CombatSpeed { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "Combat Experience Multiplier", minValue: 0f, maxValue: 10f, Order = 5, RequireRestart = false, HintText = "Multiply global experience points gained for everyone.")]
        [SettingPropertyGroup(groupName: "SendAllTroops")]
        public float Battle_SendAllTroops_XPMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "Strength of Number", minValue: 0f, maxValue: 1f, Order = 0, RequireRestart = false, HintText = "Controls the penalty applied to the side with more members. A higher value makes the side with more members stronger.")]
        [SettingPropertyGroup(groupName: "SendAllTroops/Advanced Settings", order: 6)]
        public float Battle_SendAllTroops_StrengthOfNumber { get; set; } = 0.6f;

        [SettingPropertyFloatingInteger(displayName: "Siege Strength of Number", minValue: 0f, maxValue: 1f, Order = 1, RequireRestart = false, HintText = "Controls the penalty applied to the side with more members in sieges. A higher value makes the side with more members stronger.")]
        [SettingPropertyGroup(groupName: "SendAllTroops/Advanced Settings")]
        public float Battle_SendAllTroops_SiegeStrengthOfNumber { get; set; } = 0.2f;


        [SettingPropertyBool(displayName: "WarStomp", Order = 0, RequireRestart = true, HintText = "Enable WarStomp.")]
        [SettingPropertyGroup(groupName: "WarStomp", order: 2, isMainToggle: true)]
        public bool Battle_WarStomp { get; set; } = false;

        [SettingPropertyBool(displayName: "Unstoppable War Horse Charge", Order = 1, RequireRestart = false, HintText = "Whether warhorse charges are stoppable by thrust attacks.")]
        [SettingPropertyGroup(groupName: "WarStomp")]
        public bool Battle_WarStomp_UnstoppableWarHorseCharge { get; set; } = true;

        [SettingPropertyBool(displayName: "Unstoppable Other Horse Charge", Order = 2, RequireRestart = false, HintText = "Whether other types of horse and camel charges are stoppable by thrust attacks.")]
        [SettingPropertyGroup(groupName: "WarStomp")]
        public bool Battle_WarStomp_UnstoppableHorseCharge { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "Damage Multiplier to Horse", minValue: 0f, maxValue: 10f, Order = 3, RequireRestart = false, HintText = "The damage multiplier to horses from pierce attacks.")]
        [SettingPropertyGroup(groupName: "WarStomp")]
        public float Battle_WarStomp_DamageMultiplierToHorse { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "WarStomp Damage Multiplier", minValue: 0f, maxValue: 10f, Order = 4, RequireRestart = false, HintText = "Charge damage multiplier when charging from the back.")]
        [SettingPropertyGroup(groupName: "WarStomp")]
        public float Battle_WarStomp_WarStompDamageMultiplier { get; set; } = 4.0f;


        [SettingPropertyBool(displayName: "PowerThrust", Order = 0, RequireRestart = true, HintText = "Enable PowerThrust. The total magnitude = kE * (0.5 * weight * speed^2) + kP * (weight * speed) + kC.")]
        [SettingPropertyGroup(groupName: "PowerThrust", order: 3, isMainToggle: true)]
        public bool Battle_PowerThrust { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "kE", minValue: 0f, maxValue: 0.2f, valueFormat: "0.000", Order = 1, RequireRestart = false, HintText = "The coefficient for energy when calculating magnitude.")]
        [SettingPropertyGroup(groupName: "PowerThrust")]
        public float Battle_PowerThrust_kE { get; set; } = 0.125f;

        [SettingPropertyFloatingInteger(displayName: "kP", minValue: 0f, maxValue: 0.2f, valueFormat: "0.000", Order = 2, RequireRestart = false, HintText = "The coefficient for momentum when calculating magnitude.")]
        [SettingPropertyGroup(groupName: "PowerThrust")]
        public float Battle_PowerThrust_kP { get; set; } = 0.1f;

        [SettingPropertyFloatingInteger(displayName: "kC", minValue: 0f, maxValue: 0.2f, valueFormat: "0.000", Order = 3, RequireRestart = false, HintText = "The constant coefficient when calculating magnitude.")]
        [SettingPropertyGroup(groupName: "PowerThrust")]
        public float Battle_PowerThrust_kC { get; set; } = 0.067f;

        [SettingPropertyFloatingInteger(displayName: "Thrust Hit With Arm Damage Multiplier", minValue: 0f, maxValue: 1f, Order = 4, RequireRestart = false, HintText = "Damage multiplier when it is the handguard part of the thrust weapon that hit enemy.")]
        [SettingPropertyGroup(groupName: "PowerThrust")]
        public float Battle_PowerThrust_ThrustHitWithArmDamageMultiplier { get; set; } = 0.15f;

        [SettingPropertyFloatingInteger(displayName: "Non Tip Thrust Hit Damage Multiplier", minValue: 0f, maxValue: 1f, Order = 5, RequireRestart = false, HintText = "Damage multiplier when it is the not the tip of the spear that hit enemy.")]
        [SettingPropertyGroup(groupName: "PowerThrust")]
        public float Battle_PowerThrust_NonTipThrustHitDamageMultiplier { get; set; } = 0.15f;


        [SettingPropertyBool(displayName: "RealisticBallistics", Order = 0, RequireRestart = true, HintText = "Enable RealisticBallistics.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics", order: 4, isMainToggle: true)]
        public bool Battle_RealisticBallistics { get; set; } = false;

        [SettingPropertyBool(displayName: "Consistant Arrow Speed", Order = 1, RequireRestart = false, HintText = "Change the missile speed of every bow and crossbow to match its damage.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics")]
        public bool Battle_RealisticBallistics_ConsistantArrowSpeed { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "Arrow Speed Multiplier", minValue: 0.1f, maxValue: 2f, Order = 0, RequireRestart = false, HintText = "Multiply the missile speed of arrows.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Bows", order: 2)]
        public float Battle_RealisticBallistics_ArrowSpeedMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "Bow Accuracy Multiplier", minValue: 0.1f, maxValue: 2f, Order = 1, RequireRestart = false, HintText = "Multiple the accuracy of bows.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Bows")]
        public float Battle_RealisticBallistics_BowAccuracyMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "Bow Damage Multiplier", minValue: 0.1f, maxValue: 2f, Order = 2, RequireRestart = false, HintText = "Multiple the damage of bows.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Bows")]
        public float Battle_RealisticBallistics_BowDamageMultiplier { get; set; } = 1.0f;

        [SettingPropertyBool(displayName: "Bow to Cut", Order = 3, RequireRestart = false, HintText = "Change the damage type of all bows and arrows to cut.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Bows")]
        public bool Battle_RealisticBallistics_BowToCut { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "Bolt Speed Multiplier", minValue: 0.1f, maxValue: 2f, Order = 0, RequireRestart = false, HintText = "Multiple the missile speed of bolts.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Crossbows", order: 3)]
        public float Battle_RealisticBallistics_BoltSpeedMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "Crossbow Accuracy Multiplier", minValue: 0.1f, maxValue: 2f, Order = 1, RequireRestart = false, HintText = "Multiple the accuracy of crossbows.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Crossbows")]
        public float Battle_RealisticBallistics_CrossbowAccuracyMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "Crossbow Damage Multiplier", minValue: 0.1f, maxValue: 2f, Order = 2, RequireRestart = false, HintText = "Multiple the damage of crossbows.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Crossbows")]
        public float Battle_RealisticBallistics_CrossbowDamageMultiplier { get; set; } = 1.0f;

        [SettingPropertyBool(displayName: "Crossbow to Cut", Order = 3, RequireRestart = false, HintText = "Change the damage type of all crossbows and bolts to cut.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Crossbows")]
        public bool Battle_RealisticBallistics_CrossbowToCut { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "Thrown Speed Multiplier", minValue: 0.1f, maxValue: 2f, Order = 0, RequireRestart = false, HintText = "Multiple the missile speed of thrown weapons.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Thrown Weapons", order: 4)]
        public float Battle_RealisticBallistics_ThrownSpeedMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "Thrown Accuracy Multiplier", minValue: 0.1f, maxValue: 2f, Order = 1, RequireRestart = false, HintText = "Multiple the accuracy of thrown weapons.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Thrown Weapons")]
        public float Battle_RealisticBallistics_ThrownAccuracyMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "Thrown Damage Multiplier", minValue: 0.1f, maxValue: 2f, Order = 2, RequireRestart = false, HintText = "Multiple the damage of thrown weapons.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Thrown Weapons")]
        public float Battle_RealisticBallistics_ThrownDamageMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "Air Friction of Javelins", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 0, RequireRestart = false, HintText = "The air friction coefficient of javelins. The higher the value, the shorter range they can fly.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Air Frictions", order: 5)]
        public float Battle_RealisticBallistics_AirFrictionJavelin { get; set; } = 0.002f;

        [SettingPropertyFloatingInteger(displayName: "Air Friction of Arrows and Bolts", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 1, RequireRestart = false, HintText = "The air friction coefficient of arrows and bolts. The higher the value, the shorter range they can fly.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Air Frictions")]
        public float Battle_RealisticBallistics_AirFrictionArrow { get; set; } = 0.003f;

        [SettingPropertyFloatingInteger(displayName: "Air Friction of Knives", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 2, RequireRestart = false, HintText = "The air friction coefficient of throwing knives. The higher the value, the shorter range they can fly.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Air Frictions")]
        public float Battle_RealisticBallistics_AirFrictionKnife { get; set; } = 0.007f;

        [SettingPropertyFloatingInteger(displayName: "Air Friction of Axes", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 3, RequireRestart = false, HintText = "The air friction coefficient of throwing axes. The higher the value, the shorter range they can fly.")]
        [SettingPropertyGroup(groupName: "RealisticBallistics/Air Frictions")]
        public float Battle_RealisticBallistics_AirFrictionAxe { get; set; } = 0.007f;


        [SettingPropertyBool(displayName: "Uninterrupted", Order = 0, RequireRestart = true, HintText = "Enable Uninterrupted.")]
        [SettingPropertyGroup(groupName: "Uninterrupted", order: 5, isMainToggle: true)]
        public bool Battle_Uninterrupted { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "Damage Interrupt Attack Threshold Pierce", minValue: 0f, maxValue: 50f, valueFormat: "0.0", Order = 1, RequireRestart = false, HintText = "Required pierce damage to interrupt attacks.")]
        [SettingPropertyGroup(groupName: "Uninterrupted")]
        public float Battle_Uninterrupted_DamageInterruptAttackthresholdPierce { get; set; } = 5.0f;

        [SettingPropertyFloatingInteger(displayName: "Damage Interrupt Attack Threshold Cut", minValue: 0f, maxValue: 50f, valueFormat: "0.0", Order = 2, RequireRestart = false, HintText = "Required cut damage to interrupt attacks.")]
        [SettingPropertyGroup(groupName: "Uninterrupted")]
        public float Battle_Uninterrupted_DamageInterruptAttackthresholdCut { get; set; } = 5.0f;

        [SettingPropertyFloatingInteger(displayName: "Damage Interrupt Attack Threshold Blunt", minValue: 0f, maxValue: 50f, valueFormat: "0.0", Order = 3, RequireRestart = false, HintText = "Required blunt damage to interrupt attacks.")]
        [SettingPropertyGroup(groupName: "Uninterrupted")]
        public float Battle_Uninterrupted_DamageInterruptAttackthresholdBlunt { get; set; } = 5.0f;



        [SettingPropertyBool(displayName: "ModifyRespawnParty", Order = 0, RequireRestart = true, HintText = "Enable ModifyRespawnParty.")]
        [SettingPropertyGroup(groupName: "ModifyRespawnParty", order: 6, isMainToggle: true)]
        public bool Strategy_ModifyRespawnParty { get; set; } = false;

        [SettingPropertyInteger(displayName: "AI Lord Party Size on Respawn", minValue: 0, maxValue: 200, Order = 1, RequireRestart = false, HintText = "The number of soldiers AI lords respawn with.")]
        [SettingPropertyGroup(groupName: "ModifyRespawnParty")]
        public int Strategy_ModifyRespawnParty_AILordPartySizeOnRespawn { get; set; } = 3;

        [SettingPropertyInteger(displayName: "Player Clan Party Size on Respawn", minValue: 0, maxValue: 200, Order = 2, RequireRestart = false, HintText = "The number of soldiers player's clan members start with when creating party.")]
        [SettingPropertyGroup(groupName: "ModifyRespawnParty")]
        public int Strategy_ModifyRespawnParty_PlayerPartySizeOnRespawn { get; set; } = 0;


        [SettingPropertyBool(displayName: "LearnToQuit", Order = 0, RequireRestart = true, HintText = "Enable LearnToQuit.")]
        [SettingPropertyGroup(groupName: "LearnToQuit", order: 7, isMainToggle: true)]
        public bool Strategy_LearnToQuit { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "Retreat Chance Multiplier", minValue: 0f, maxValue: 5f, Order = 1, RequireRestart = false, HintText = "Multiplier to the chance of successful retreat.")]
        [SettingPropertyGroup(groupName: "LearnToQuit")]
        public float Strategy_LearnToQuit_RetreatChance { get; set; } = 1.0f;

        [SettingPropertyBool(displayName: "Verbose", Order = 2, RequireRestart = false, HintText = "Turn on the \"Lord retreat from battle\" message.")]
        [SettingPropertyGroup(groupName: "LearnToQuit")]
        public bool Strategy_LearnToQuit_Verbose { get; set; } = true;


        [SettingPropertyBool(displayName: "BanditMerger", Order = 0, RequireRestart = true, HintText = "Enable BanditMerger.")]
        [SettingPropertyGroup(groupName: "BanditMerger", order: 8, isMainToggle: true)]
        public bool Strategy_BanditMerger { get; set; } = false;

        [SettingPropertyInteger(displayName: "Merge Radius", minValue: 0, maxValue: 100, Order = 1, RequireRestart = false, HintText = "The distance between two bandit groups to be merged.")]
        [SettingPropertyGroup(groupName: "BanditMerger")]
        public int Strategy_BanditMerger_MergeRadius { get; set; } = 15;

        [SettingPropertyInteger(displayName: "Max Non Looter Number For Merge", minValue: 0, maxValue: 300, Order = 2, RequireRestart = false, HintText = "The maximum number of non-looter bandits in the same group to be allowed to merge more.")]
        [SettingPropertyGroup(groupName: "BanditMerger")]
        public int Strategy_BanditMerger_MaxNonLooterNumberForMerge { get; set; } = 20;

        [SettingPropertyInteger(displayName: "Max Looter Number For Merge", minValue: 0, maxValue: 300, Order = 3, RequireRestart = false, HintText = "The maximum number of looters in the same group to be allowed to merge more.")]
        [SettingPropertyGroup(groupName: "BanditMerger")]
        public int Strategy_BanditMerger_MaxLooterNumberForMerge { get; set; } = 20;

        [SettingPropertyInteger(displayName: "Maximum Looter Parties", minValue: 0, maxValue: 500, Order = 4, RequireRestart = false, HintText = "The maximum total number of looter parties.")]
        [SettingPropertyGroup(groupName: "BanditMerger")]
        public int Strategy_BanditMerger_MaximumLooterParties { get; set; } = 100;
    }
}
