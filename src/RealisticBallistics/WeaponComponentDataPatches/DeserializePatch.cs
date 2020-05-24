using HarmonyLib;
using TaleWorlds.Core;

namespace CombatModCollection.RealisticBallistics.WeaponComponentDataPatches
{
    [HarmonyPatch(typeof(WeaponComponentData), "Deserialize")]
    public class DeserializePatch
    {
        public static void Postfix(ref WeaponComponentData __instance)
        {
            WeaponPatcher.PatchWeapon(__instance);
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_RealisticBallistics;
        }
    }
}
