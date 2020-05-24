using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection.WarStomp.MissionPatches
{
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
            return SubModule.Settings.Battle_WarStomp;
        }
    }
}
