using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;

namespace CombatModCollection
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => "Light.CombatModCollection_v1";
        public override string DisplayName => "Combat Mod Collection";
        public override string FolderName => "CombatModCollection";


        [SettingPropertyBool(displayName: "{=TUkzZ4}SurviveByArmor", Order = 0, RequireRestart = true, HintText = "{=qbaoGX}Enable SurviveByArmor.")]
        [SettingPropertyGroup(groupName: "{=TUkzZ4}SurviveByArmor", GroupOrder = 0, IsMainToggle = true)]
        public bool Battle_SurviveByArmor { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "{=G8UbtR}Blunt Death Rate", minValue: 0f, maxValue: 1f, Order = 1, RequireRestart = false, HintText = "{=aASlgg}The death rate of blunt weapons.")]
        [SettingPropertyGroup(groupName: "{=TUkzZ4}SurviveByArmor")]
        public float Battle_SurviveByArmor_BluntDeathRate { get; set; } = 0.0f;

        [SettingPropertyBool(displayName: "{=FFtdLT}Survive By ExcessiveDamage", Order = 0, RequireRestart = false, HintText = "{=z5oYR8}Use excessive damage to determine survival rate. Excessive damage is the excessive part of the killing blow.")]
        [SettingPropertyGroup(groupName: "{=TUkzZ4}SurviveByArmor/{=yWuPAT}Survive By Excessive Damage", GroupOrder = 2, IsMainToggle = true)]
        public bool Battle_SurviveByArmor_SurviveByExcessiveDamage { get; set; } = true;

        [SettingPropertyInteger(displayName: "{=Nbv5e2}Safe Excessive Damage", minValue: -50, maxValue: 50, Order = 1, RequireRestart = false, HintText = "{=o7jTMR}If excessive damage is within this value, one is guaranteed to survive.")]
        [SettingPropertyGroup(groupName: "{=TUkzZ4}SurviveByArmor/{=yWuPAT}Survive By Excessive Damage")]
        public int Battle_SurviveByArmor_SafeExcessiveDamage { get; set; } = 0;

        [SettingPropertyInteger(displayName: "{=dteLyD}Lethal Excessive Damage", minValue: 0, maxValue: 100, Order = 2, RequireRestart = false, HintText = "{=qLy4nz}If excessive damage is beyond this value, one is guaranteed to die unless saved by surgeon.")]
        [SettingPropertyGroup(groupName: "{=TUkzZ4}SurviveByArmor/{=yWuPAT}Survive By Excessive Damage")]
        public int Battle_SurviveByArmor_LethalExcessiveDamage { get; set; } = 30;

        [SettingPropertyBool(displayName: "{=1CyVUE}Apply Medicine Bonus", Order = 3, RequireRestart = false, HintText = "{=G1miJd}Apply survival bonus from the medicine level of party surgeon.")]
        [SettingPropertyGroup(groupName: "{=TUkzZ4}SurviveByArmor/{=yWuPAT}Survive By Excessive Damage")]
        public bool Battle_SurviveByArmor_ApplyMedicine { get; set; } = true;

        [SettingPropertyBool(displayName: "{=S0TV5w}Apply Level Bonus", Order = 4, RequireRestart = false, HintText = "{=ufMrjL}Apply survival bonus from one's character level.")]
        [SettingPropertyGroup(groupName: "{=TUkzZ4}SurviveByArmor/{=yWuPAT}Survive By Excessive Damage")]
        public bool Battle_SurviveByArmor_ApplyLevel { get; set; } = false;

        [SettingPropertyBool(displayName: "{=2IqGzK}Apply Armor Bonus", Order = 5, RequireRestart = false, HintText = "{=UOU4JL}Apply survival bonus from one's armor value.")]
        [SettingPropertyGroup(groupName: "{=TUkzZ4}SurviveByArmor/{=yWuPAT}Survive By Excessive Damage")]
        public bool Battle_SurviveByArmor_ApplyArmor { get; set; } = false;

        [SettingPropertyBool(displayName: "{=9D1mkX}Survive By Armor Value", Order = 0, RequireRestart = false, HintText = "{=AFJ03H}Use Armor value instead of character level to determine survival chance when Survive By Excessive Damage is off, or in battle simulations.")]
        [SettingPropertyGroup(groupName: "{=TUkzZ4}SurviveByArmor/{=9D1mkX}Survive By Armor Value", GroupOrder = 3, IsMainToggle = true)]
        public bool Battle_SurviveByArmor_SurviveByArmorValue { get; set; } = true;

        [SettingPropertyInteger(displayName: "{=sSG34d}Armor Value Threshold", minValue: 1, maxValue: 400, Order = 1, RequireRestart = false, HintText = "{=ivmSs1}The armor value threshold to give a medium survival chance.")]
        [SettingPropertyGroup(groupName: "{=TUkzZ4}SurviveByArmor/{=9D1mkX}Survive By Armor Value")]
        public int Battle_SurviveByArmor_ArmorValueThreshold { get; set; } = 100;


        [SettingPropertyBool(displayName: "{=ITpg8n}SendAllTroops", Order = 0, RequireRestart = true, HintText = "{=myZoTQ}Enable SendAllTroops.")]
        [SettingPropertyGroup(groupName: "{=ITpg8n}SendAllTroops", GroupOrder = 1, IsMainToggle = true)]
        public bool Battle_SendAllTroops { get; set; } = false;

        [SettingPropertyBool(displayName: "{=4aqVhB}Random Damage", Order = 1, RequireRestart = false, HintText = "{=ZrqDEF}If set to false, the damage value is fixed. If true, it will randomly fluctuate around the center points.")]
        [SettingPropertyGroup(groupName: "{=ITpg8n}SendAllTroops")]
        public bool Battle_SendAllTroops_RandomDamage { get; set; } = false;

        [SettingPropertyBool(displayName: "{=DGJxvz}Random Death", Order = 2, RequireRestart = false, HintText = "{=ex9Bix}If set to false, the death of soldier is determined by hitpoints reach 0. If true, the vanilla instant-death-by-chance is used.")]
        [SettingPropertyGroup(groupName: "{=ITpg8n}SendAllTroops")]
        public bool Battle_SendAllTroops_RandomDeath { get; set; } = false;

        [SettingPropertyBool(displayName: "{=na7hDa}Detailed Combat Model", Order = 3, RequireRestart = false, HintText = "{=ReaYvH}Whether to use equipments and skill levels of a soldier to determine his strength, and simulate the progression of battles. If set to false, the vanilla Power is used.")]
        [SettingPropertyGroup(groupName: "{=ITpg8n}SendAllTroops")]
        public bool Battle_SendAllTroops_DetailedCombatModel { get; set; } = true;

        [SettingPropertyFloatingInteger(displayName: "{=h1rTcI}Combat Speed", minValue: 0f, maxValue: 10f, Order = 4, RequireRestart = false, HintText = "{=A4sffk}Multiply the combat speed between AIs.")]
        [SettingPropertyGroup(groupName: "{=ITpg8n}SendAllTroops")]
        public float Battle_SendAllTroops_CombatSpeed { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "{=IALEm6}Combat Experience Multiplier", minValue: 0f, maxValue: 10f, Order = 5, RequireRestart = false, HintText = "{=rMT98d}Multiply global experience points gained for everyone.")]
        [SettingPropertyGroup(groupName: "{=ITpg8n}SendAllTroops")]
        public float Battle_SendAllTroops_XPMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "{=cO5EoB}Strength of Number", minValue: 0f, maxValue: 1f, Order = 0, RequireRestart = false, HintText = "{=Kg9oy2}Controls the penalty applied to the side with more members. A higher value makes the side with more members stronger.")]
        [SettingPropertyGroup(groupName: "{=ITpg8n}SendAllTroops/{=l7RZ2J}Advanced Settings", GroupOrder = 6)]
        public float Battle_SendAllTroops_StrengthOfNumber { get; set; } = 0.6f;

        [SettingPropertyFloatingInteger(displayName: "{=TopudX}Siege Strength of Number", minValue: 0f, maxValue: 1f, Order = 1, RequireRestart = false, HintText = "{=9djOfH}Controls the penalty applied to the side with more members in sieges. A higher value makes the side with more members stronger.")]
        [SettingPropertyGroup(groupName: "{=ITpg8n}SendAllTroops/{=l7RZ2J}Advanced Settings")]
        public float Battle_SendAllTroops_SiegeStrengthOfNumber { get; set; } = 0.2f;


        [SettingPropertyBool(displayName: "{=1RYg7H}WarStomp", Order = 0, RequireRestart = true, HintText = "{=9XnXjr}Enable WarStomp.")]
        [SettingPropertyGroup(groupName: "{=1RYg7H}WarStomp", GroupOrder = 2, IsMainToggle = true)]
        public bool Battle_WarStomp { get; set; } = false;

        [SettingPropertyBool(displayName: "{=b218nG}Unstoppable War Horse Charge", Order = 1, RequireRestart = false, HintText = "{=hMCreW}Whether warhorse charges are stoppable by thrust attacks.")]
        [SettingPropertyGroup(groupName: "{=1RYg7H}WarStomp")]
        public bool Battle_WarStomp_UnstoppableWarHorseCharge { get; set; } = true;

        [SettingPropertyBool(displayName: "{=Xe8k75}Unstoppable Other Horse Charge", Order = 2, RequireRestart = false, HintText = "{=R78PyF}Whether other types of horse and camel charges are stoppable by thrust attacks.")]
        [SettingPropertyGroup(groupName: "{=1RYg7H}WarStomp")]
        public bool Battle_WarStomp_UnstoppableHorseCharge { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "{=4tLPR7}Damage Multiplier to Horse", minValue: 0f, maxValue: 10f, Order = 3, RequireRestart = false, HintText = "{=IIwiJr}The damage multiplier to horses from pierce attacks.")]
        [SettingPropertyGroup(groupName: "{=1RYg7H}WarStomp")]
        public float Battle_WarStomp_DamageMultiplierToHorse { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "{=mtsOlw}WarStomp Damage Multiplier", minValue: 0f, maxValue: 10f, Order = 4, RequireRestart = false, HintText = "{=dNcNpx}Charge damage multiplier when charging from the back.")]
        [SettingPropertyGroup(groupName: "{=1RYg7H}WarStomp")]
        public float Battle_WarStomp_WarStompDamageMultiplier { get; set; } = 4.0f;


        [SettingPropertyBool(displayName: "{=4s3EnO}PowerThrust", Order = 0, RequireRestart = true, HintText = "{=IH4Opq}Enable PowerThrust. The total magnitude = kE * (0.5 * weight * speed^2) + kP * (weight * speed) + kC.")]
        [SettingPropertyGroup(groupName: "{=4s3EnO}PowerThrust", GroupOrder = 3, IsMainToggle = true)]
        public bool Battle_PowerThrust { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "{=6HDWN0}kE", minValue: 0f, maxValue: 0.2f, valueFormat: "0.000", Order = 1, RequireRestart = false, HintText = "{=DJD6ps}The coefficient for energy when calculating magnitude.")]
        [SettingPropertyGroup(groupName: "{=4s3EnO}PowerThrust")]
        public float Battle_PowerThrust_kE { get; set; } = 0.125f;

        [SettingPropertyFloatingInteger(displayName: "{=GbJfTq}kP", minValue: 0f, maxValue: 0.2f, valueFormat: "0.000", Order = 2, RequireRestart = false, HintText = "{=kqjPv1}The coefficient for momentum when calculating magnitude.")]
        [SettingPropertyGroup(groupName: "{=4s3EnO}PowerThrust")]
        public float Battle_PowerThrust_kP { get; set; } = 0.1f;

        [SettingPropertyFloatingInteger(displayName: "{=ym5ln0}kC", minValue: 0f, maxValue: 0.2f, valueFormat: "0.000", Order = 3, RequireRestart = false, HintText = "{=B9z8Rf}The constant coefficient when calculating magnitude.")]
        [SettingPropertyGroup(groupName: "{=4s3EnO}PowerThrust")]
        public float Battle_PowerThrust_kC { get; set; } = 0.067f;

        [SettingPropertyFloatingInteger(displayName: "{=EWbPbo}Thrust Hit With Arm Damage Multiplier", minValue: 0f, maxValue: 1f, Order = 4, RequireRestart = false, HintText = "{=HCeN37}Damage multiplier when it is the handguard part of the thrust weapon that hit enemy.")]
        [SettingPropertyGroup(groupName: "{=4s3EnO}PowerThrust")]
        public float Battle_PowerThrust_ThrustHitWithArmDamageMultiplier { get; set; } = 0.15f;

        [SettingPropertyFloatingInteger(displayName: "{=rQSUc5}Non Tip Thrust Hit Damage Multiplier", minValue: 0f, maxValue: 1f, Order = 5, RequireRestart = false, HintText = "{=NzBZEU}Damage multiplier when it is the not the tip of the spear that hit enemy.")]
        [SettingPropertyGroup(groupName: "{=4s3EnO}PowerThrust")]
        public float Battle_PowerThrust_NonTipThrustHitDamageMultiplier { get; set; } = 0.15f;


        [SettingPropertyBool(displayName: "{=GUT10Z}RealisticBallistics", Order = 0, RequireRestart = true, HintText = "{=346ex2}Enable RealisticBallistics.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics", GroupOrder = 4, IsMainToggle = true)]
        public bool Battle_RealisticBallistics { get; set; } = false;

        [SettingPropertyBool(displayName: "{=j7cmtP}Consistant Arrow Speed", Order = 1, RequireRestart = false, HintText = "{=r3x53T}Change the missile speed of every bow and crossbow to match its damage.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics")]
        public bool Battle_RealisticBallistics_ConsistantArrowSpeed { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "{=f0MQ6b}Arrow Speed Multiplier", minValue: 0.1f, maxValue: 2f, Order = 0, RequireRestart = false, HintText = "{=cxEZom}Multiply the missile speed of arrows.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=ZK8HFi}Bows", GroupOrder = 2)]
        public float Battle_RealisticBallistics_ArrowSpeedMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "{=0xKaDd}Bow Accuracy Multiplier", minValue: 0.1f, maxValue: 2f, Order = 1, RequireRestart = false, HintText = "{=5T1Oir}Multiple the accuracy of bows.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=ZK8HFi}Bows")]
        public float Battle_RealisticBallistics_BowAccuracyMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "{=MwDSUM}Bow Damage Multiplier", minValue: 0.1f, maxValue: 2f, Order = 2, RequireRestart = false, HintText = "{=wt8yxn}Multiple the damage of bows.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=ZK8HFi}Bows")]
        public float Battle_RealisticBallistics_BowDamageMultiplier { get; set; } = 1.0f;

        [SettingPropertyBool(displayName: "{=Mu5Csz}Bow to Cut", Order = 3, RequireRestart = false, HintText = "{=XdfLfX}Change the damage type of all bows and arrows to cut.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=ZK8HFi}Bows")]
        public bool Battle_RealisticBallistics_BowToCut { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "{=u97Mjd}Bolt Speed Multiplier", minValue: 0.1f, maxValue: 2f, Order = 0, RequireRestart = false, HintText = "{=A29Q87}Multiple the missile speed of bolts.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=wWbzF5}Crossbows", GroupOrder = 3)]
        public float Battle_RealisticBallistics_BoltSpeedMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "{=RS9Q5r}Crossbow Accuracy Multiplier", minValue: 0.1f, maxValue: 2f, Order = 1, RequireRestart = false, HintText = "{=0TxJ7V}Multiple the accuracy of crossbows.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=wWbzF5}Crossbows")]
        public float Battle_RealisticBallistics_CrossbowAccuracyMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "{=O6Def8}Crossbow Damage Multiplier", minValue: 0.1f, maxValue: 2f, Order = 2, RequireRestart = false, HintText = "{=EHRZB4}Multiple the damage of crossbows.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=wWbzF5}Crossbows")]
        public float Battle_RealisticBallistics_CrossbowDamageMultiplier { get; set; } = 1.0f;

        [SettingPropertyBool(displayName: "{=hYsrxk}Crossbow to Cut", Order = 3, RequireRestart = false, HintText = "{=ToDlUG}Change the damage type of all crossbows and bolts to cut.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=wWbzF5}Crossbows")]
        public bool Battle_RealisticBallistics_CrossbowToCut { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "{=FkTyl1}Thrown Speed Multiplier", minValue: 0.1f, maxValue: 2f, Order = 0, RequireRestart = false, HintText = "{=fAlgaM}Multiple the missile speed of thrown weapons.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=OwEq8g}Thrown Weapons", GroupOrder = 4)]
        public float Battle_RealisticBallistics_ThrownSpeedMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "{=hEepCK}Thrown Accuracy Multiplier", minValue: 0.1f, maxValue: 2f, Order = 1, RequireRestart = false, HintText = "{=4wpBgt}Multiple the accuracy of thrown weapons.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=OwEq8g}Thrown Weapons")]
        public float Battle_RealisticBallistics_ThrownAccuracyMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "{=Afhn07}Thrown Damage Multiplier", minValue: 0.1f, maxValue: 2f, Order = 2, RequireRestart = false, HintText = "{=vvOGJR}Multiple the damage of thrown weapons.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=OwEq8g}Thrown Weapons")]
        public float Battle_RealisticBallistics_ThrownDamageMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger(displayName: "{=vtk5qm}Air Friction of Javelins", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 0, RequireRestart = false, HintText = "{=sIzIQe}The air friction coefficient of javelins. The higher the value, the shorter range they can fly.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=uM0htR}Air Frictions", GroupOrder = 5)]
        public float Battle_RealisticBallistics_AirFrictionJavelin { get; set; } = 0.002f;

        [SettingPropertyFloatingInteger(displayName: "{=UwsNpv}Air Friction of Arrows and Bolts", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 1, RequireRestart = false, HintText = "{=drgPfb}The air friction coefficient of arrows and bolts. The higher the value, the shorter range they can fly.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=uM0htR}Air Frictions")]
        public float Battle_RealisticBallistics_AirFrictionArrow { get; set; } = 0.003f;

        [SettingPropertyFloatingInteger(displayName: "{=M0SuVr}Air Friction of Knives", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 2, RequireRestart = false, HintText = "{=Kd4VmF}The air friction coefficient of throwing knives. The higher the value, the shorter range they can fly.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=uM0htR}Air Frictions")]
        public float Battle_RealisticBallistics_AirFrictionKnife { get; set; } = 0.007f;

        [SettingPropertyFloatingInteger(displayName: "{=LyggxT}Air Friction of Axes", minValue: 0f, maxValue: 0.02f, valueFormat: "0.000", Order = 3, RequireRestart = false, HintText = "{=wMEqS6}The air friction coefficient of throwing axes. The higher the value, the shorter range they can fly.")]
        [SettingPropertyGroup(groupName: "{=GUT10Z}RealisticBallistics/{=uM0htR}Air Frictions")]
        public float Battle_RealisticBallistics_AirFrictionAxe { get; set; } = 0.007f;


        [SettingPropertyBool(displayName: "{=93jkRp}Uninterrupted", Order = 0, RequireRestart = true, HintText = "{=9PV6v8}Enable Uninterrupted.")]
        [SettingPropertyGroup(groupName: "{=93jkRp}Uninterrupted", GroupOrder = 5, IsMainToggle = true)]
        public bool Battle_Uninterrupted { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "{=ivwMpr}Damage Interrupt Attack Threshold Pierce", minValue: 0f, maxValue: 50f, valueFormat: "0.0", Order = 1, RequireRestart = false, HintText = "{=H5ck0h}Required pierce damage to interrupt attacks.")]
        [SettingPropertyGroup(groupName: "{=93jkRp}Uninterrupted")]
        public float Battle_Uninterrupted_DamageInterruptAttackthresholdPierce { get; set; } = 5.0f;

        [SettingPropertyFloatingInteger(displayName: "{=QReRZg}Damage Interrupt Attack Threshold Cut", minValue: 0f, maxValue: 50f, valueFormat: "0.0", Order = 2, RequireRestart = false, HintText = "{=noUcuQ}Required cut damage to interrupt attacks.")]
        [SettingPropertyGroup(groupName: "{=93jkRp}Uninterrupted")]
        public float Battle_Uninterrupted_DamageInterruptAttackthresholdCut { get; set; } = 5.0f;

        [SettingPropertyFloatingInteger(displayName: "{=u9nr4f}Damage Interrupt Attack Threshold Blunt", minValue: 0f, maxValue: 50f, valueFormat: "0.0", Order = 3, RequireRestart = false, HintText = "{=j7CCyc}Required blunt damage to interrupt attacks.")]
        [SettingPropertyGroup(groupName: "{=93jkRp}Uninterrupted")]
        public float Battle_Uninterrupted_DamageInterruptAttackthresholdBlunt { get; set; } = 5.0f;



        [SettingPropertyBool(displayName: "{=4ssQXo}ModifyRespawnParty", Order = 0, RequireRestart = true, HintText = "{=3DEgLf}Enable ModifyRespawnParty.")]
        [SettingPropertyGroup(groupName: "{=4ssQXo}ModifyRespawnParty", GroupOrder = 6, IsMainToggle = true)]
        public bool Strategy_ModifyRespawnParty { get; set; } = false;

        [SettingPropertyInteger(displayName: "{=S6aWgf}AI Lord Party Size on Respawn", minValue: 0, maxValue: 200, Order = 1, RequireRestart = false, HintText = "{=mWy5M3}The number of soldiers AI lords respawn with.")]
        [SettingPropertyGroup(groupName: "{=4ssQXo}ModifyRespawnParty")]
        public int Strategy_ModifyRespawnParty_AILordPartySizeOnRespawn { get; set; } = 3;

        [SettingPropertyInteger(displayName: "{=V2Wjgt}Player Clan Party Size on Respawn", minValue: 0, maxValue: 200, Order = 2, RequireRestart = false, HintText = "{=Yq2nnC}The number of soldiers player's clan members start with when creating party.")]
        [SettingPropertyGroup(groupName: "{=4ssQXo}ModifyRespawnParty")]
        public int Strategy_ModifyRespawnParty_PlayerPartySizeOnRespawn { get; set; } = 0;


        [SettingPropertyBool(displayName: "{=IO6I6z}LearnToQuit", Order = 0, RequireRestart = true, HintText = "{=uTgEDC}Enable LearnToQuit.")]
        [SettingPropertyGroup(groupName: "{=IO6I6z}LearnToQuit", GroupOrder = 7, IsMainToggle = true)]
        public bool Strategy_LearnToQuit { get; set; } = false;

        [SettingPropertyFloatingInteger(displayName: "{=l1OFVA}Retreat Chance Multiplier", minValue: 0f, maxValue: 5f, Order = 1, RequireRestart = false, HintText = "{=grJG1p}Multiplier to the chance of successful retreat.")]
        [SettingPropertyGroup(groupName: "{=IO6I6z}LearnToQuit")]
        public float Strategy_LearnToQuit_RetreatChance { get; set; } = 1.0f;

        [SettingPropertyBool(displayName: "{=KxWZN4}Verbose", Order = 2, RequireRestart = false, HintText = "{=X60XwQ}Turn on the party retreating from battle message.")]
        [SettingPropertyGroup(groupName: "{=IO6I6z}LearnToQuit")]
        public bool Strategy_LearnToQuit_Verbose { get; set; } = true;


        [SettingPropertyBool(displayName: "{=XzbHdq}BanditMerger", Order = 0, RequireRestart = true, HintText = "{=qGI8by}Enable BanditMerger.")]
        [SettingPropertyGroup(groupName: "{=XzbHdq}BanditMerger", GroupOrder = 8, IsMainToggle = true)]
        public bool Strategy_BanditMerger { get; set; } = false;

        [SettingPropertyInteger(displayName: "{=nwr2tB}Merge Radius", minValue: 0, maxValue: 100, Order = 1, RequireRestart = false, HintText = "{=ZZOPqG}The distance between two bandit groups to be merged.")]
        [SettingPropertyGroup(groupName: "{=XzbHdq}BanditMerger")]
        public int Strategy_BanditMerger_MergeRadius { get; set; } = 12;

        [SettingPropertyInteger(displayName: "{=TGaCVr}Max Non Looter Number For Merge", minValue: 0, maxValue: 300, Order = 2, RequireRestart = false, HintText = "{=gPtfQt}The maximum number of non-looter bandits in the same group to be allowed to merge more.")]
        [SettingPropertyGroup(groupName: "{=XzbHdq}BanditMerger")]
        public int Strategy_BanditMerger_MaxNonLooterNumberForMerge { get; set; } = 20;

        [SettingPropertyInteger(displayName: "{=w2mfQR}Max Looter Number For Merge", minValue: 0, maxValue: 300, Order = 3, RequireRestart = false, HintText = "{=EBW4fg}The maximum number of looters in the same group to be allowed to merge more.")]
        [SettingPropertyGroup(groupName: "{=XzbHdq}BanditMerger")]
        public int Strategy_BanditMerger_MaxLooterNumberForMerge { get; set; } = 20;

        [SettingPropertyInteger(displayName: "{=g8Wb7m}Maximum Looter Parties", minValue: 0, maxValue: 500, Order = 4, RequireRestart = false, HintText = "{=wT4K5t}The maximum total number of looter parties.")]
        [SettingPropertyGroup(groupName: "{=XzbHdq}BanditMerger")]
        public int Strategy_BanditMerger_MaximumLooterParties { get; set; } = 100;
    }
}