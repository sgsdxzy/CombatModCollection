using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace CombatModCollection
{
    class LightBanditDensityModel : DefaultBanditDensityModel
    {
        public override int NumberOfMaximumLooterParties
        {
            get
            {
                return (int)Settings.Instance.Strategy_BanditMerger_MaximumLooterParties;
            }
        }

        /*
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
        */
    }
}
