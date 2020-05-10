using HarmonyLib;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection
{
    public class SubModule : MBSubModuleBase
    {
        public static Settings Settings { get; private set; }

        private static void LoadSettings()
        {
            string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.json");
            try
            {
                SubModule.Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(configPath));
            }
            catch
            { }
        }


        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            SubModule.LoadSettings();
            var harmony = new Harmony("mod.bannerlord.lightcombat");
            harmony.PatchAll();
        }


        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            AddModels(gameStarterObject as CampaignGameStarter);
            AddBehaviors(gameStarterObject as CampaignGameStarter);
        }


        private void AddModels(CampaignGameStarter gameStarter)
        {
            if (Settings.Strategy_BanditMerger)
            {
                gameStarter?.AddModel(new LightBanditDensityModel());
            }
        }


        private void AddBehaviors(CampaignGameStarter gameStarter)
        {
            if (Settings.Strategy_BanditMerger)
            {
                gameStarter?.AddBehavior(new BanditMergeBehavior());
            }
        }
    }
}