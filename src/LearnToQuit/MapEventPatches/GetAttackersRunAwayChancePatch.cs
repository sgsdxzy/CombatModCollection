using HarmonyLib;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CombatModCollection.LearnToQuit.MapEventPatches
{
    [HarmonyPatch(typeof(MapEvent), "GetAttackersRunAwayChance")]
    public class GetAttackersRunAwayChancePatch
    {
        public static bool Prefix(ref bool __result, MapEvent __instance, int ____mapEventUpdateCount)
        {
            if (____mapEventUpdateCount <= 1)
            {
                __result = false;
                return false;
            }

            bool AttackerRunaway = false;
            bool DefenderRunaway = false;

            // __instance.SimulateBattleSetup();
            MapEventSide attackerSide = __instance.AttackerSide;
            MapEventSide defenderSide = __instance.DefenderSide;
            float attackerTotalStrength = attackerSide.RecalculateStrengthOfSide();
            float defenderTotalStrength = defenderSide.RecalculateStrengthOfSide();

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
                    float baseChance = (powerRatio - 1.2f) * 1.5f * Settings.Instance.Strategy_LearnToQuit_RetreatChance;
                    float bonus = -(float)__instance.AttackerSide.LeaderParty.LeaderHero.GetTraitLevel(DefaultTraits.Valor) * 0.2f;
                    AttackerRunaway = MBRandom.RandomFloat < baseChance + bonus;
                }
            }
            if (AttackerRunaway)
            {
                if (Settings.Instance.Strategy_LearnToQuit_Verbose)
                {
                    Dictionary<string, TextObject> attributes = new Dictionary<string, TextObject>
                    {
                        {"ATTACKER", __instance.AttackerSide.LeaderParty.Name },
                        {"DEFENDER", __instance.DefenderSide.LeaderParty.Name },
                    };
                    TextObject information = new TextObject("{=dJhAtk}{ATTACKER} withdrew from battle against {DEFENDER}.", attributes);
                    InformationManager.DisplayMessage(new InformationMessage(information.ToString()));
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
                    int forTryingToGetAway = TroopSacrificeModel.GetNumberOfTroopsSacrificedForTryingToGetAway(__instance.DefenderSide, __instance.AttackerSide, 1 / powerRatio, ofRegularMembers);
                    if (forTryingToGetAway < 0 || ofRegularMembers < forTryingToGetAway)
                    {
                        // Not enough man
                        DefenderRunaway = false;
                    }
                    else
                    {
                        sacrificeRatio = (float)forTryingToGetAway / (float)ofRegularMembers;
                        float baseChance = (1f - 1.25f * powerRatio) * 1.5f * Settings.Instance.Strategy_LearnToQuit_RetreatChance;
                        float bonus = -(float)__instance.DefenderSide.LeaderParty.LeaderHero.GetTraitLevel(DefaultTraits.Valor) * 0.2f;
                        DefenderRunaway = MBRandom.RandomFloat < baseChance + bonus;
                    }
                }
            }
            if (DefenderRunaway)
            {
                MapEventCustomMembers.DefendersRanAway[__instance.Id] = true;
                if (Settings.Instance.Strategy_LearnToQuit_Verbose)
                {
                    Dictionary<string, TextObject> attributes = new Dictionary<string, TextObject>
                    {
                        {"ATTACKER", __instance.AttackerSide.LeaderParty.Name },
                        {"DEFENDER", __instance.DefenderSide.LeaderParty.Name },
                    };
                    TextObject information = new TextObject("{=LLIPq6}{DEFENDER} was forced to retreat against {ATTACKER}.", attributes);
                    InformationManager.DisplayMessage(new InformationMessage(information.ToString()));
                }
                TroopSacrificeModel.SacrificeTroops(sacrificeRatio, __instance.DefenderSide, __instance);

                __result = true;
                return false;
            }

            __result = false;
            return false;
        }

        public static bool Prepare()
        {
            return Settings.Instance.Strategy_LearnToQuit;
        }
    }
}