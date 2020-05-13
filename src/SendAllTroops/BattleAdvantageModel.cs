using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CombatModCollection.SendAllTroops
{
    class BattleAdvantageModel
    {
        public static float PartyBattleAdvantage(PartyBase party)
        {
            float num = 1f;
            if (party.LeaderHero != null)
            {
                int skillValue = party.LeaderHero.GetSkillValue(DefaultSkills.Tactics);
                num += (float)((double)DefaultSkillEffects.TacticsAdvantage.PrimaryBonus * (double)skillValue * 0.01);
            }
            return num;
        }
    }
}
