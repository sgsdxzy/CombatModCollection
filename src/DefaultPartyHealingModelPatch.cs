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
            __result = SurvivalModel.GetSurvivalChance(party, character, damageType, enemyParty);

            return false;
        }

        public static bool Prepare()
        {
            return (SubModule.Settings.Battle_SurviveByArmor && SubModule.Settings.Battle_SurviveByArmor_SurviveByExcessiveDamage) || SubModule.Settings.Battle_GoodSoildersNeverDie;
        }
    }
}
