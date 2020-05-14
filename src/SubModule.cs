using HarmonyLib;
using System.IO;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection
{
    public class SubModule : MBSubModuleBase
    {
        //public static Settings Settings { get; private set; }
        private bool Patched = false;

        private static void LoadSettings()
        {
            string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.json");
            try
            {
                //Settings.Instance = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(configPath));
            }
            catch
            { }
        }


        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            //SubModule.LoadSettings();
            //var harmony = new Harmony("mod.bannerlord.lightcombat");
            //harmony.PatchAll();
            // bool a = Settings.Instance.Battle_PowerThrust;
        }

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