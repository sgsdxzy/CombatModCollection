using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(Mission), "ComputeBlowMagnitudeFromHorseCharge")]
    public class ComputeBlowMagnitudeFromHorseChargePatch
    {
        private const double NonZero = 1e-5;

        public static void Postfix(ref AttackCollisionData acd,
            Vec3 attackerAgentMovementDirection,
            Vec3 attackerAgentVelocity,
            float agentMountChargeDamageProperty,
            Vec3 victimAgentVelocity,
            Vec3 victimAgentPosition,
            ref float baseMagnitude,
            ref float specialMagnitude)
        {
            if (attackerAgentVelocity.Length > NonZero && victimAgentVelocity.Length > NonZero)
            {
                if (Vec3.DotProduct(attackerAgentVelocity, victimAgentVelocity) /
                    (attackerAgentVelocity.Length * victimAgentVelocity.Length) > 0.5)
                {
                    specialMagnitude *= SubModule.Settings.Battle_WarStomp_WarStompDamageMultiplier;
                }
            }

        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_WarStomp && SubModule.Settings.Battle_WarStomp_WarStompDamageMultiplier != 1.0f;
        }
    }

    [HarmonyPatch(typeof(Mission), "ComputeBlowDamage")]
    public class ComputeBlowDamagePatch
    {
        public static void Postfix(float armorAmountFloat,
            WeaponComponentData shieldOnBack,
            AgentFlag victimAgentFlag,
            float victimAgentAbsorbedDamageRatio,
            float damageMultiplierOfBone,
            float combatDifficultyMultiplier,
            DamageTypes damageType,
            float magnitude,
            Vec3 blowPosition,
            ItemObject item,
            bool blockedWithShield,
            bool hitShieldOnBack,
            int speedBonus,
            bool cancelDamage,
            bool isFallDamage,
            ref int inflictedDamage,
            ref int absorbedByArmor,
            ref int armorAmount)
        {
            if (damageType == DamageTypes.Pierce && victimAgentFlag.HasAnyFlag<AgentFlag>(AgentFlag.Mountable))
            {
                inflictedDamage = (int)(inflictedDamage * SubModule.Settings.Battle_WarStomp_DamageMultiplierToHorse);
            }
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_WarStomp && SubModule.Settings.Battle_WarStomp_DamageMultiplierToHorse != 1.0f;
        }
    }

    [HarmonyPatch(typeof(Mission), "CreateHorseAgentFromRosterElements")]
    public class CreateHorseAgentFromRosterElementsPatch
    {
        public static void Postfix(ref Agent __result,
            EquipmentElement horse,
            EquipmentElement monsterHarness,
            MatrixFrame initialFrame,
            int forcedAgentMountIndex,
            string horseCreationKey)
        {
            if (SubModule.Settings.Battle_WarStomp_UnstoppableHorseCharge ||
                (SubModule.Settings.Battle_WarStomp_UnstoppableWarHorseCharge && horse.Item.ItemCategory == DefaultItemCategories.WarHorse))
            {
                var flags = __result.GetAgentFlags();
                flags &= ~AgentFlag.CanRear;
                __result.SetAgentFlags(flags);
            }
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_WarStomp;
        }
    }
}
