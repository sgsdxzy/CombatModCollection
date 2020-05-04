using TaleWorlds.CampaignSystem;

namespace CombatModCollection
{
    class LightBanditDensityModel : BanditDensityModel
    {
        public override int NumberOfMaximumLooterParties
        {
            get
            {
                return (int)SubModule.Settings.Strategy_BanditMerger_MaximumBanditParties;
            }
        }

        public override int NumberOfMinimumBanditPartiesInAHideoutToInfestIt
        {
            get
            {
                return 2;
            }
        }

        public override int NumberOfMaximumBanditPartiesInEachHideout
        {
            get
            {
                return 3;
            }
        }

        public override int NumberOfMaximumBanditPartiesAroundEachHideout
        {
            get
            {
                return 4;
            }
        }

        public override int NumberOfMaximumHideoutsAtEachBanditFaction
        {
            get
            {
                return 8;
            }
        }

        public override int NumberOfInitialHideoutsAtEachBanditFaction
        {
            get
            {
                return 2;
            }
        }
    }
}
