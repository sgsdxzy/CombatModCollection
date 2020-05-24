using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection
{
    public class SubModule : MBSubModuleBase
    {
        private bool Patched = false;
        public static Settings Settings;

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            if (!Patched)
            {
                Settings = Settings.Instance;
                var harmony = new Harmony("mod.bannerlord.lightcombat");
                harmony.PatchAll();
                Patched = true;
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            AddModels(gameStarterObject as CampaignGameStarter);
            AddBehaviors(gameStarterObject as CampaignGameStarter);
        }

        private void AddModels(CampaignGameStarter gameStarter)
        {
            if (SubModule.Settings.Strategy_BanditMerger)
            {
                gameStarter?.AddModel(new BanditMerger.LightBanditDensityModel());
            }
        }

        private void AddBehaviors(CampaignGameStarter gameStarter)
        {
            if (SubModule.Settings.Strategy_BanditMerger)
            {
                gameStarter?.AddBehavior(new BanditMerger.BanditMergeBehavior());
            }
        }
    }
}