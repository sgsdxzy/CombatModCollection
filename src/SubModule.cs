using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using HarmonyLib;
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
            {
            }
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            SubModule.LoadSettings();
            var harmony = new Harmony("mod.bannerlord.lightcombat");
            harmony.PatchAll();
        }
    }
}