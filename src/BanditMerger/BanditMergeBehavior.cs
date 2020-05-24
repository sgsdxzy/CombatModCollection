using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace CombatModCollection.BanditMerger
{
    class BanditMergeBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener((object)this, new Action(this.DailyTick));
        }

        public override void SyncData(IDataStore dataStore)
        { }

        public void DailyTick()
        {
            foreach (MobileParty mobileParty in MobileParty.All.ToList())
            {
                if (mobileParty != null && mobileParty.IsActive && mobileParty.IsBandit && !mobileParty.IsCurrentlyUsedByAQuest
                    && mobileParty.CurrentSettlement == null && mobileParty.MapEvent == null
                    && NumberCanBeMerged(mobileParty))
                {
                    foreach (MobileParty nearbyMobileParty in Campaign.Current.GetNearbyMobileParties(
                        mobileParty.Position2D, SubModule.Settings.Strategy_BanditMerger_MergeRadius, (Func<MobileParty, bool>)(x => true)).ToList())
                    {
                        if (nearbyMobileParty != null && nearbyMobileParty != mobileParty && nearbyMobileParty.IsActive
                            && nearbyMobileParty.IsBandit && !nearbyMobileParty.IsCurrentlyUsedByAQuest
                            && nearbyMobileParty.CurrentSettlement == null && nearbyMobileParty.MapEvent == null
                            && nearbyMobileParty.MapFaction.StringId == mobileParty.MapFaction.StringId
                            && NumberCanBeMerged(nearbyMobileParty))
                        {
                            mobileParty.Party.AddMembers(nearbyMobileParty.MemberRoster.ToFlattenedRoster());
                            mobileParty.Party.AddPrisoners(nearbyMobileParty.PrisonRoster.ToFlattenedRoster());
                            DestroyPartyAction.Apply(mobileParty.Party, nearbyMobileParty);
                        }
                    }
                }
            }
        }

        private bool NumberCanBeMerged(MobileParty party)
        {
            if (party.MapFaction.StringId == "looters")
            {
                return party.Party.NumberOfAllMembers < SubModule.Settings.Strategy_BanditMerger_MaxLooterNumberForMerge;
            }
            else
            {
                return party.Party.NumberOfAllMembers < SubModule.Settings.Strategy_BanditMerger_MaxNonLooterNumberForMerge;
            }
        }
    }
}
