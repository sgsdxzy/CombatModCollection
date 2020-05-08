﻿using System.Collections.Concurrent;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection
{

    public class MapEventState
    {
        private static readonly FieldInfo MapEvent__mapEventUpdateCount = typeof(MapEvent).GetField(
            "_mapEventUpdateCount", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly ConcurrentDictionary<MBGUID, MapEventState> AllMapEventStates = new ConcurrentDictionary<MBGUID, MapEventState>();

        public static MapEventState GetMapEventState(MapEvent mapEvent)
        {
            if (!AllMapEventStates.TryGetValue(mapEvent.Id, out MapEventState mapEventState))
            {
                mapEventState = new MapEventState();
                AllMapEventStates[mapEvent.Id] = mapEventState;
                if (SubModule.Settings.Battle_SendAllTroops)
                {
                    mapEventState.StageRounds = (int)MapEvent__mapEventUpdateCount.GetValue(mapEvent);
                    mapEventState.isSiege = mapEvent.IsSiegeAssault;
                }
            }
            return mapEventState;
        }

        public static void RemoveMapEventState(MapEvent mapEvent)
        {
            AllMapEventStates.TryRemove(mapEvent.Id, out _);
        }


        private readonly ConcurrentDictionary<string, PartyState> PartyStates = new ConcurrentDictionary<string, PartyState>();
        private bool isSiege = false;
        public int BattleScale = 2;
        public int StageRounds = 0;
        public bool IsDefenderRunAway = false;

        private PartyState GetPartyState(PartyBase party)
        {
            if (!PartyStates.TryGetValue(party.Id, out PartyState partyState))
            {
                partyState = new PartyState(this);
                PartyStates[party.Id] = partyState;
                for (int index = 0; index < party.MemberRoster.Count; ++index)
                {
                    TroopRosterElement elementCopyAtIndex = party.MemberRoster.GetElementCopyAtIndex(index);
                    CharacterObject troop = elementCopyAtIndex.Character;
                    if (troop != null)
                    {
                        partyState.RegisterTroops(troop, (elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber), isSiege);
                    }                       
                }
            }
            return partyState;
        }

        public float GetPartyStrength(PartyBase party)
        {
            PartyState partyState = GetPartyState(party);
            return partyState.GetCurrentStrength();
        }

        public AttackComposition GetPartyAttack(PartyBase party)
        {
            PartyState partyState = GetPartyState(party);
            return partyState.GetPartyAttack();
        }

        public bool ApplyDamageToPartyTroop(AttackComposition attack, PartyBase party, CharacterObject troop, out float damage)
        {
            PartyState partyState = GetPartyState(party);
            return partyState.ApplyDamageToTroop(attack, troop, out damage);
        }

        public AttackComposition GetAttack(PartyBase party, CharacterObject troop)
        {
            PartyState partyState = GetPartyState(party);
            return partyState.GetAttack(troop);
        }
    }
}