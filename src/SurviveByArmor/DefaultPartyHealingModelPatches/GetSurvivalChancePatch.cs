﻿using HarmonyLib;
using Helpers;
using System;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CombatModCollection.SurviveByArmor.DefaultPartyHealingModelPatches
{
    [HarmonyPatch(typeof(DefaultPartyHealingModel), "GetSurvivalChance")]
    public class GetSurvivalChancePatch
    {
        public static float GetExcessiveDamageDeathRate(float excessiveDamage)
        {
            float deathRate = (excessiveDamage - SubModule.Settings.Battle_SurviveByArmor_SafeExcessiveDamage)
                / (SubModule.Settings.Battle_SurviveByArmor_LethalExcessiveDamage - SubModule.Settings.Battle_SurviveByArmor_SafeExcessiveDamage);

            deathRate = Math.Max(deathRate, 0);

            return deathRate;
        }

        public static bool Prefix(ref float __result,
            PartyBase party,
            CharacterObject character,
            DamageTypes damageType,
            PartyBase enemyParty = null)
        {
            if (character.IsHero)
            {
                if (character.HeroObject.AlwaysDie)
                {
                    __result = 0.0f;
                    return false;
                }
                if (character.HeroObject.AlwaysUnconscious)
                {
                    __result = 1f;
                    return false;
                }
            }

            bool hasDamageData = BasicCharacterObjectCustomMembers.ExcessiveDamages.TryRemove(character.Id, out float excessiveDamage);
            float damageTypeDeathRate = damageType == DamageTypes.Blunt ?
                SubModule.Settings.Battle_SurviveByArmor_BluntDeathRate : 1.0f;

            bool useMedicine;
            bool useLevel;
            bool useArmor;
            float baseDeathRate;
            if (SubModule.Settings.Battle_SurviveByArmor_SurviveByExcessiveDamage && hasDamageData)
            {
                baseDeathRate = GetExcessiveDamageDeathRate(excessiveDamage);
                useMedicine = SubModule.Settings.Battle_SurviveByArmor_ApplyMedicine;
                useLevel = SubModule.Settings.Battle_SurviveByArmor_ApplyLevel;
                useArmor = SubModule.Settings.Battle_SurviveByArmor_ApplyArmor;
            }
            else
            {
                baseDeathRate = 1.0f;
                useMedicine = true;
                useLevel = !SubModule.Settings.Battle_SurviveByArmor_SurviveByArmorValue;
                useArmor = SubModule.Settings.Battle_SurviveByArmor_SurviveByArmorValue;
            }

            ExplainedNumber stat = new ExplainedNumber(character.IsHero ? 10f : 1f, (StringBuilder)null);
            if (party != null && party.MobileParty != null)
            {
                if (useMedicine)
                {
                    SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.SurgeonSurvivalBonus, party.MobileParty, ref stat);
                    if (enemyParty?.MobileParty != null && enemyParty.MobileParty.HasPerk(DefaultPerks.Medicine.DoctorsOath))
                        SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.SurgeonSurvivalBonus, enemyParty.MobileParty, ref stat);
                    if (character.IsHero && (party.MobileParty.Leader?.HeroObject != null && party.MobileParty.LeaderHero != character.HeroObject))
                        PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.FortitudeTonic, party.MobileParty, ref stat);
                }
                if (useArmor)
                {
                    try
                    {
                        float head = character.GetHeadArmorSum();
                        float body = character.GetBodyArmorSum();
                        float arm = character.GetArmArmorSum();
                        float leg = character.GetLegArmorSum();
                        float totalArmor = head + body + arm + leg;
                        stat.Add(totalArmor / SubModule.Settings.Battle_SurviveByArmor_ArmorValueThreshold * 16f * 0.03f, (TextObject)null);
                    }
                    catch (NullReferenceException)
                    {
                        // Cannot find FirstBattleEquipment for the troop, possible the troop is added by other mods
                        stat.Add((float)character.Level * 0.03f, (TextObject)null);
                    }
                }
                if (useLevel)
                {
                    stat.Add((float)character.Level * 0.03f, (TextObject)null);
                }
            }
            float deathRate = damageTypeDeathRate * baseDeathRate / stat.ResultNumber;

            __result = 1.0f - deathRate;
            if (__result < 0f) __result = 0f;
            if (__result > 1f) __result = 1f;

            return false;
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SurviveByArmor;
        }
    }
}
