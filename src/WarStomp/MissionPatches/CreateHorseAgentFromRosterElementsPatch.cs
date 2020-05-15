using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CombatModCollection.WarStomp.MissionPatches
{
    [HarmonyPatch(typeof(Mission), "CreateHorseAgentFromRosterElements")]
    public class CreateHorseAgentFromRosterElementsPatch
    {
        public static void Postfix(ref Agent __result,
            EquipmentElement horse,
            EquipmentElement monsterHarness,
            MatrixFrame initialFrame,
            int forcedAgentMountIndex,
            string horseCreationKey)
        {
            if (Settings.Instance.Battle_WarStomp_UnstoppableHorseCharge ||
                (Settings.Instance.Battle_WarStomp_UnstoppableWarHorseCharge && horse.Item.ItemCategory == DefaultItemCategories.WarHorse))
            {
                var flags = __result.GetAgentFlags();
                flags &= ~AgentFlag.CanRear;
                __result.SetAgentFlags(flags);
            }
        }

        public static bool Prepare()
        {
            return Settings.Instance.Battle_WarStomp;
        }
    }
}
