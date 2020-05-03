using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Library;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(DefaultCombatXpModel), "GetXpFromHit")]
    public class GetXpFromHitPatch
    {
        public static bool Prefix(CharacterObject attackerTroop,
            CharacterObject attackedTroop,
            int damage,
            bool isFatal,
            CombatXpModel.MissionTypeEnum missionType,
            out int xpAmount)
        {
            int val2 = 100;
            double num1;
            if (attackedTroop == null)
            {
                num1 = 0.400000005960464 * 2.0 * Math.Min(damage, val2);
            }
            else
            {
                val2 = attackedTroop.MaxHitPoints();
                num1 = 0.400000005960464 * (((double)attackedTroop.GetPower() + 0.5) * (double)(Math.Min(damage, val2) + (isFatal ? val2 : 0)));
            }
            double num2;
            switch (missionType)
            {
                case CombatXpModel.MissionTypeEnum.Battle:
                    num2 = 1.0;
                    break;
                case CombatXpModel.MissionTypeEnum.PracticeFight:
                    num2 = 1.0 / 16.0;
                    break;
                case CombatXpModel.MissionTypeEnum.Tournament:
                    num2 = 0.330000013113022;
                    break;
                case CombatXpModel.MissionTypeEnum.SimulationBattle:
                    num2 = 0.899999976158142 * SubModule.Settings.Battle_SendAllTroops_SimXPMultiplier;
                    break;
                case CombatXpModel.MissionTypeEnum.NoXp:
                    num2 = 0.0;
                    break;
                default:
                    num2 = 1.0;
                    break;
            }
            float f = (float)(num1 * num2);
            xpAmount = MathF.Round(f);

            return false;
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Battle_SendAllTroops;
        }
    }
}
