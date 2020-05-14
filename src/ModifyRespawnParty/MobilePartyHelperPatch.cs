using HarmonyLib;
using Helpers;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection
{
    [HarmonyPatch(typeof(MobilePartyHelper), "SpawnLordPartyInternal")]
    public class SpawnLordPartyInternalPatch
    {
        private static readonly MethodInfo OnLordPartySpawnedMI = typeof(CampaignEventDispatcher).GetMethod("OnLordPartySpawned", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        public static bool Prefix(ref MobileParty __result, Hero hero,
            Vec2 position,
            float spawnRadius,
            Settlement spawnSettlement)
        {
            MobileParty spawnedParty = MBObjectManager.Instance.CreateObject<MobileParty>(hero.CharacterObject.StringId + "_" + (object)hero.NumberOfCreatedParties);
            ++hero.NumberOfCreatedParties;
            spawnedParty.AddElementToMemberRoster(hero.CharacterObject, 1, true);
            int troopNumberLimit = hero == Hero.MainHero || hero.Clan == Clan.PlayerClan ?
                (int)Settings.Instance.Strategy_ModifyRespawnParty_PlayerPartySizeOnRespawn : (int)Settings.Instance.Strategy_ModifyRespawnParty_AILordPartySizeOnRespawn;
            if (!Campaign.Current.GameStarted)
            {
                float num = (float)(1.0 - (double)MBRandom.RandomFloat * (double)MBRandom.RandomFloat);
                troopNumberLimit = (int)((double)spawnedParty.Party.PartySizeLimit * (double)num);
            }
            TextObject partyName = MobilePartyHelper.GeneratePartyName((BasicCharacterObject)hero.CharacterObject);
            spawnedParty.InitializeMobileParty(partyName, hero.Clan.DefaultPartyTemplate, position, spawnRadius, 0.0f, MobileParty.PartyTypeEnum.Lord, troopNumberLimit);
            spawnedParty.Party.Owner = hero;
            spawnedParty.IsLordParty = true;
            spawnedParty.Party.Visuals.SetMapIconAsDirty();
            if (spawnSettlement != null)
                spawnedParty.SetMoveGoToSettlement(spawnSettlement);
            spawnedParty.Aggressiveness = (float)(0.899999976158142 + 0.100000001490116 * (double)hero.GetTraitLevel(DefaultTraits.Valor) - 0.0500000007450581 * (double)hero.GetTraitLevel(DefaultTraits.Mercy));
            hero.PassedTimeAtHomeSettlement = (float)(int)((double)MBRandom.RandomFloat * 100.0);
            if (spawnSettlement != null)
            {
                spawnedParty.Ai.SetAIState(AIState.VisitingNearbyTown, (PartyBase)null);
                spawnedParty.SetMoveGoToSettlement(spawnSettlement);
            }

            // CampaignEventDispatcher.Instance.OnLordPartySpawned(spawnedParty);
            object[] parametersArray = new object[] { spawnedParty };
            OnLordPartySpawnedMI.Invoke(CampaignEventDispatcher.Instance, parametersArray);

            __result = spawnedParty;

            return false;
        }

        public static bool Prepare()
        {
            return Settings.Instance.Strategy_ModifyRespawnParty;
        }
    }
}
