using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace CombatModCollection.LearnToQuit.MapEventPatches
{
    [HarmonyPatch(typeof(MapEvent), "FinishBattle")]
    public class FinishBattlePatch
    {
        private static readonly FieldInfo MapEvent__attackersRanAway = typeof(MapEvent).GetField(
            "_attackersRanAway", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        public static void Prefix(MapEvent __instance)
        {
            if (MapEventCustomMembers.DefendersRanAway.TryRemove(__instance.Id, out _))
            {
                // Defender ran away
                // this._attackersRanAway = false;
                MapEvent__attackersRanAway.SetValue(__instance, false);

                // Place denfenders further away from attackers to prevent instantly get caught again
                foreach (PartyBase party in (IEnumerable<PartyBase>)__instance.DefenderSide.Parties)
                {
                    if (party.IsMobile)
                    {
                        MobileParty mobileParty = party.MobileParty;
                        if (mobileParty.IsActive && mobileParty.AttachedTo == null)
                        {
                            if (mobileParty.BesiegerCamp != null && mobileParty.BesiegerCamp.SiegeParties.Contains<PartyBase>(mobileParty.Party))
                                mobileParty.BesiegerCamp.RemoveSiegeParty(mobileParty);
                            Vec2 pointAroundPosition = mobileParty.FindReachablePointAroundPosition(mobileParty.Position2D, 3.1f, 3f, true);
                            mobileParty.Position2D = pointAroundPosition;
                            mobileParty.SetMoveModeHold();
                            mobileParty.IgnoreForHours(0.5f);
                        }
                    }
                }
            }
        }

        public static bool Prepare()
        {
            return SubModule.Settings.Strategy_LearnToQuit;
        }
    }
}