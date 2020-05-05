using Helpers;
using System;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection
{
    class SurvivalModel
    {
        public static float GetSurvivalChance(
            PartyBase party,
            CharacterObject character,
            DamageTypes damageType,
            PartyBase enemyParty = null,
            bool isSimulation = false)
        {
            if (character.IsHero)
            {
                if (character.HeroObject.AlwaysDie)
                {
                    return 0.0f;
                }
                if (character.HeroObject.AlwaysUnconscious)
                {
                    return 1f;
                }
            }
            if (damageType == DamageTypes.Blunt)
            {
                return 1f;
            }
            ExplainedNumber stat = new ExplainedNumber(character.IsHero ? 10f : 1f, (StringBuilder)null);
            if (party != null && party.MobileParty != null)
            {
                SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.SurgeonSurvivalBonus, party.MobileParty, ref stat);
                if (enemyParty?.MobileParty != null && enemyParty.MobileParty.HasPerk(DefaultPerks.Medicine.DoctorsOath))
                    SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.SurgeonSurvivalBonus, enemyParty.MobileParty, ref stat);

                if (isSimulation || !(SubModule.Settings.Battle_SurviveByArmor && SubModule.Settings.Battle_SurviveByArmor_SurviveByExcessiveDamage))
                {
                    // If SubModule.Settings.Battle_SurviveByArmor_SurviveByExcessiveDamage is true, the actual survival chance is calculated 
                    // elsewhere and the result is in the form of damageType
                    // Simulations always have this bonus
                    if (SubModule.Settings.Battle_SurviveByArmor && SubModule.Settings.Battle_SurviveByArmor_SurviveByArmorValue)
                    {
                        try
                        {
                            float head = character.GetHeadArmorSum();
                            float body = character.GetBodyArmorSum();
                            float arm = character.GetArmArmorSum();
                            float leg = character.GetLegArmorSum();
                            float totalArmor = head + body + arm + leg;
                            stat.Add(totalArmor / SubModule.Settings.Battle_SurviveByArmor_ArmorValueThreshold * 16 * 0.03f, (TextObject)null);
                        }
                        catch (NullReferenceException)
                        {
                            // Cannot find FirstBattleEquipment for the troop, possible added by other mods
                            stat.Add((float)character.Level * 0.03f, (TextObject)null);
                        }
                    }
                    else
                    {
                        stat.Add((float)character.Level * 0.03f, (TextObject)null);
                    }
                }

                if (character.IsHero && (party.MobileParty.Leader?.HeroObject != null && party.MobileParty.LeaderHero != character.HeroObject))
                    PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.FortitudeTonic, party.MobileParty, ref stat);
            }
            float deathRate = 1.0f / stat.ResultNumber;
            if (SubModule.Settings.Battle_GoodSoildersNeverDie)
            {
                if (party == PartyBase.MainParty || !SubModule.Settings.Battle_GoodSoildersNeverDie_OnlyApplyToPlayerParty)
                {
                    if ((float)character.Level >= SubModule.Settings.Battle_GoodSoildersNeverDie_MinimumLevel)
                    {
                        deathRate *= SubModule.Settings.Battle_GoodSoildersNeverDie_DeathRate;
                    }
                }
            }
            return 1.0f - deathRate;
        }


        public static float GetExcessiveDamageSurvivalChance(Agent agent, float excessiveDamage)
        {
            float chance = 1 - (excessiveDamage - SubModule.Settings.Battle_SurviveByArmor_SafeExcessiveDamage)
                / (SubModule.Settings.Battle_SurviveByArmor_LethalExcessiveDamage - SubModule.Settings.Battle_SurviveByArmor_SafeExcessiveDamage);

            return chance;
        }
    }
}
