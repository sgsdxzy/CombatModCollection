using Helpers;
using System;
using System.Linq;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CombatModCollection.LearnToQuit
{
    internal class TroopSacrificeModel
    {
        // PlayerEncounter.SacrificeTroops
        public static void SacrificeTroops(float ratio, MapEventSide side, MapEvent mapEvent)
        {
            // side.MakeReadyForSimulation();
            foreach (PartyBase party in side.Parties)
            {
                SacrificeTroopsWithRatio(party.MobileParty, ratio);
            }
        }

        private static void SacrificeTroopsWithRatio(
            MobileParty mobileParty,
            float sacrifaceRatio)
        {
            int num1 = MBRandom.RoundRandomized((float)mobileParty.Party.NumberOfRegularMembers * sacrifaceRatio);
            for (int index = 0; index < num1; ++index)
            {
                float num2 = 100f;
                TroopRosterElement troopRosterElement1 = mobileParty.Party.MemberRoster.First<TroopRosterElement>();
                foreach (TroopRosterElement troopRosterElement2 in mobileParty.Party.MemberRoster)
                {
                    float num3 = (float)((double)troopRosterElement2.Character.Level - (troopRosterElement2.WoundedNumber > 0 ? 0.5 : 0.0) - (double)MBRandom.RandomFloat * 0.5);
                    if (!troopRosterElement2.Character.IsHero && (double)num3 < (double)num2 && troopRosterElement2.Number > 0)
                    {
                        num2 = num3;
                        troopRosterElement1 = troopRosterElement2;
                    }
                }
                mobileParty.MemberRoster.AddToCounts(troopRosterElement1.Character, -1, false, troopRosterElement1.WoundedNumber > 0 ? -1 : 0, 0, true, -1);
            }
        }

        // DefaultTroopSacrificeModel.GetNumberOfTroopsSacrificedForTryingToGetAway
        public static int GetNumberOfTroopsSacrificedForTryingToGetAway(
            MapEventSide mapEventSide,
            MapEventSide oppositeSide,
            float powerRatio,
            int ofRegularMembers)
        {
            int num2 = mapEventSide.CountTroops((Func<FlattenedTroopRosterElement, bool>)(x => x.State == RosterTroopState.Active && !x.Troop.IsHero));
            ExplainedNumber stat = new ExplainedNumber(1f, (StringBuilder)null);
            if (mapEventSide.LeaderParty.Leader != null)
            {
                SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Tactics, DefaultSkillEffects.TacticsTroopSacrificeReduction, mapEventSide.LeaderParty.Leader, ref stat, true);
            }

            int num3 = Math.Max(((double)ofRegularMembers * Math.Pow((double)Math.Min(powerRatio, 3f), 1.29999995231628) * 0.100000001490116 / (2.0 / (2.0 + ((double)stat.ResultNumber - 1.0) * 10.0)) + 5.0).Round(), 1);
            return num3 <= num2 ? num3 : -1;
        }
    }
}
