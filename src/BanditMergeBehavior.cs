using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection
{
    class BanditMergeBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener((object)this, new Action(this.DailyTick));
        }

        public override void SyncData(IDataStore dataStore)
        {}

        public void DailyTick()
        {
            foreach (MobileParty mobileParty in MobileParty.All.ToList())
            {
                if (mobileParty != null && mobileParty.IsActive && mobileParty.IsBandit && !mobileParty.IsCurrentlyUsedByAQuest
                    && mobileParty.CurrentSettlement == null && mobileParty.MapEvent == null
                    && mobileParty.Party.NumberOfAllMembers < SubModule.Settings.Strategy_BanditMerger_MaxNumber)
                {
                    foreach (MobileParty nearbyMobileParty in Campaign.Current.GetNearbyMobileParties(
                        mobileParty.Position2D, SubModule.Settings.Strategy_BanditMerger_MergRadius, (Func<MobileParty, bool>)(x => true)).ToList())
                    {
                        if (nearbyMobileParty != null && nearbyMobileParty != mobileParty && nearbyMobileParty.IsActive
                            && nearbyMobileParty.IsBandit && !nearbyMobileParty.IsCurrentlyUsedByAQuest
                            && nearbyMobileParty.CurrentSettlement == null && nearbyMobileParty.MapEvent == null
                            && nearbyMobileParty.MapFaction.StringId == mobileParty.MapFaction.StringId
                            && nearbyMobileParty.Party.NumberOfAllMembers < SubModule.Settings.Strategy_BanditMerger_MaxNumber)
                        {
                            mobileParty.Party.AddMembers(nearbyMobileParty.MemberRoster.ToFlattenedRoster());
                            mobileParty.Party.AddPrisoners(nearbyMobileParty.PrisonRoster.ToFlattenedRoster());
                            DestroyPartyAction.Apply(mobileParty.Party, nearbyMobileParty);
                            // InformationManager.DisplayMessage(new InformationMessage("Merged: " + mobileParty.MapFaction.StringId));
                        }
                    }                    
                }
            }
        }
    }
}
