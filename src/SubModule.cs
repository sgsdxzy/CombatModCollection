using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection
{
    public class SubModule : MBSubModuleBase
    {
        private bool Patched = false;

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            if (!Patched)
            {
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
            if (Settings.Instance.Strategy_BanditMerger)
            {
                gameStarter?.AddModel(new LightBanditDensityModel());
            }
        }

        private void AddBehaviors(CampaignGameStarter gameStarter)
        {
            if (Settings.Instance.Strategy_BanditMerger)
            {
                gameStarter?.AddBehavior(new BanditMergeBehavior());
            }
        }
    }
}