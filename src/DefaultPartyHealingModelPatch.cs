using HarmonyLib;
using Helpers;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(DefaultPartyHealingModel), "GetSurvivalChance")]
    public class GetSurvivalChancePatch
    {
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
            if (damageType == DamageTypes.Blunt)
            {
                __result = 1f;
                return false;
            }
            if (SubModule.Settings.Battle_GoodSoildersNeverDie)
            {
                if (party == PartyBase.MainParty || !SubModule.Settings.Battle_GoodSoildersNeverDie_OnlyApplyToPlayerParty)
                {
                    if ((float)character.Level >= SubModule.Settings.Battle_GoodSoildersNeverDie_MinimumLevel)
                    {
                        __result = 1f;
                        return false;
                    }
                }
            }
            ExplainedNumber stat = new ExplainedNumber(character.IsHero ? 10f : 1f, (StringBuilder)null);
            if (party != null && party.MobileParty != null)
            {
                SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.SurgeonSurvivalBonus, party.MobileParty, ref stat);
                if (enemyParty?.MobileParty != null && enemyParty.MobileParty.HasPerk(DefaultPerks.Medicine.DoctorsOath))
                    SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.SurgeonSurvivalBonus, enemyParty.MobileParty, ref stat);

                // stat.Add((float)character.Level * 0.03f, (TextObject)null);
                float head = character.GetHeadArmorSum();
                float body = character.GetBodyArmorSum();
                float arm = character.GetArmArmorSum();
                float leg = character.GetLegArmorSum();
                float totalArmor = head + body + arm + leg;
                stat.Add(totalArmor / SubModule.Settings.Battle_SurviveByArmor_ArmorThreshold * 16 * 0.03f, (TextObject)null);

                if (character.IsHero && (party.MobileParty.Leader?.HeroObject != null && party.MobileParty.LeaderHero != character.HeroObject))
                    PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.FortitudeTonic, party.MobileParty, ref stat);
            }
            __result = (float)(1.0 - 1.0 / (double)stat.ResultNumber);

            return false;
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SurviveByArmor || SubModule.Settings.Battle_GoodSoildersNeverDie;
        }
    }
}
