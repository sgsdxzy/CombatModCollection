using HarmonyLib;
using TaleWorlds.Core;

namespace CombatModCollection.RealisticBallistics.WeaponComponentDataPatches
{
    [HarmonyPatch(typeof(WeaponComponentData), "Init")]
    public class InitPatch
    {
        public static void Postfix(ref WeaponComponentData __instance)
        {
            WeaponPatcher.PatchWeapon(__instance);
        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_RealisticBallistics;
        }
    }
}
