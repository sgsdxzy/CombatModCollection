using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(MapEvent), "GetAttackersRunAwayChance")]
    public class GetAttackersRunAwayChancePatch
    {
        // PlayerEncounter.SacrificeTroops
        private static void SacrificeTroops(float ratio, MapEventSide side, MapEvent mapEvent)
        {
            side.MakeReadyForSimulation();
            foreach (PartyBase party in side.Parties)
            {
                SacrifaceTroopsWithRatio(party.MobileParty, ratio);
            }
        }

        private static void SacrifaceTroopsWithRatio(
            MobileParty mobileParty,
            float sacrifaceRatio)
        {
            int num1 = MBRandom.RoundRandomized((float)mobileParty.Party.NumberOfRegularMembers * sacrifaceRatio);
            for (int index = 0; index < num1; ++index)
            {
                float num2 = 100f;
                TroopRosterElement troopRosterElement1 = mobileParty.Party.MemberRoster.First<TroopRosterElement>();
                foreach (TroopRosterElement troopRosterElement2 in mobileParty.Party.MemberRoster)
                {
                    float num3 = (float)((double)troopRosterElement2.Character.Level - (troopRosterElement2.WoundedNumber > 0 ? 0.5 : 0.0) - (double)MBRandom.RandomFloat * 0.5);
                    if (!troopRosterElement2.Character.IsHero && (double)num3 < (double)num2 && troopRosterElement2.Number > 0)
                    {
                        num2 = num3;
                        troopRosterElement1 = troopRosterElement2;
                    }
                }
                mobileParty.MemberRoster.AddToCounts(troopRosterElement1.Character, -1, false, troopRosterElement1.WoundedNumber > 0 ? -1 : 0, 0, true, -1);
            }
        }


        // DefaultTroopSacrificeModel.GetNumberOfTroopsSacrificedForTryingToGetAway
        public static int GetNumberOfTroopsSacrificedForTryingToGetAway(
            MapEventSide mapEventSide,
            MapEventSide oppositeSide,
            float powerRatio,
            int ofRegularMembers)
        {
            float val1 = powerRatio;

            int num2 = mapEventSide.CountTroops((Func<FlattenedTroopRosterElement, bool>)(x => x.State == RosterTroopState.Active && !x.Troop.IsHero));
            ExplainedNumber stat = new ExplainedNumber(1f, (StringBuilder)null);
            if (mapEventSide.LeaderParty.Leader != null)
            {
                SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Tactics, DefaultSkillEffects.TacticsTroopSacrificeReduction, mapEventSide.LeaderParty.Leader, ref stat, true);
            }

            int num3 = Math.Max(((double)ofRegularMembers * Math.Pow((double)Math.Min(val1, 3f), 1.29999995231628) * 0.100000001490116 / (2.0 / (2.0 + ((double)stat.ResultNumber - 1.0) * 10.0)) + 5.0).Round(), 1);
            return num3 <= num2 ? num3 : -1;
        }

        public static bool Prefix(ref bool __result, MapEvent __instance, int ____mapEventUpdateCount)
        {
            if (____mapEventUpdateCount <= 1)
            {
                __result = false;
                return false;
            }

            bool AttackerRunaway = false;
            bool DefenderRunaway = false;

            __instance.SimulateBattleSetup();
            MapEventSide attackerSide = __instance.AttackerSide;
            MapEventSide defenderSide = __instance.DefenderSide;
            MapEventState mapEventState = MapEventState.GetMapEventState(__instance);
            float attackerTotalStrength = MapEventSideHelper.RecalculateStrengthOfSide(attackerSide, mapEventState);
            float defenderTotalStrength = MapEventSideHelper.RecalculateStrengthOfSide(defenderSide, mapEventState);

            if (__instance.IsSiegeAssault)
            {
                attackerTotalStrength *= 0.6666667f;
            }
            float powerRatio = defenderTotalStrength / attackerTotalStrength;

            if (__instance.AttackerSide.LeaderParty.LeaderHero == null)
            {
                AttackerRunaway = false;
            }
            else
            {
                // Attacker Runaway
                if (powerRatio > 1.2)
                {
                    float baseChance = (powerRatio - 1.2f) / 1.2f;
                    float bonus = -(float)__instance.AttackerSide.LeaderParty.LeaderHero.GetTraitLevel(DefaultTraits.Valor) * 0.1f;
                    AttackerRunaway = MBRandom.RandomFloat < baseChance + bonus;
                }
            }
            if (AttackerRunaway)
            {
                if (SubModule.Settings.Strategy_LearnToQuit_Verbose)
                {
                    string information = __instance.AttackerSide.LeaderParty.Name.ToString() +
                        " withdrew from battle against " +
                        __instance.DefenderSide.LeaderParty.Name.ToString();
                    InformationManager.DisplayMessage(new InformationMessage(information));
                }

                __result = true;
                return false;
            }

            float sacrificeRatio = 0f;
            if (__instance.DefenderSide.LeaderParty.LeaderHero == null)
            {
                DefenderRunaway = false;
            }
            else
            {
                // Defender Runaway
                if (powerRatio <= 0.8f)
                {
                    int ofRegularMembers = 0;
                    foreach (PartyBase party in __instance.DefenderSide.Parties)
                    {
                        ofRegularMembers += party.NumberOfRegularMembers;
                    }
                    int forTryingToGetAway = GetNumberOfTroopsSacrificedForTryingToGetAway(__instance.DefenderSide, __instance.AttackerSide, 1 / powerRatio, ofRegularMembers);
                    if (forTryingToGetAway < 0 || ofRegularMembers < forTryingToGetAway)
                    {
                        // Not enough man
                        DefenderRunaway = false;
                    }
                    else
                    {
                        sacrificeRatio = (float)forTryingToGetAway / (float)ofRegularMembers;
                        float baseChance = (1f - 1.25f * powerRatio) * 0.8f;
                        float bonus = -(float)__instance.DefenderSide.LeaderParty.LeaderHero.GetTraitLevel(DefaultTraits.Valor) * 0.1f;
                        DefenderRunaway = MBRandom.RandomFloat < baseChance + bonus;
                    }
                }
            }
            if (DefenderRunaway)
            {
                mapEventState.IsDefenderRunAway = true;

                if (SubModule.Settings.Strategy_LearnToQuit_Verbose)
                {
                    string information = __instance.DefenderSide.LeaderParty.Name.ToString() +
                        " was forced to retreat against " +
                        __instance.AttackerSide.LeaderParty.Name.ToString();
                    InformationManager.DisplayMessage(new InformationMessage(information));
                }

                SacrificeTroops(sacrificeRatio, __instance.DefenderSide, __instance);

                __result = true;
                return false;
            }

            __result = false;
            return false;
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Strategy_LearnToQuit;
        }
    }
}