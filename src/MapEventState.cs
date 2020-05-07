using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Dynamic;
using System.Net;
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
                }
                
            }         
            return mapEventState;
        }

        public static void RemoveMapEventState(MapEvent mapEvent)
        {
            AllMapEventStates.TryRemove(mapEvent.Id, out _);
        }


        private readonly ConcurrentDictionary<string, PartyState> PartyStates = new ConcurrentDictionary<string, PartyState>();
        public int BattleScale = 2;
        public int StageRounds = 0;
        public bool firstUpdated = false;
        public bool IsDefenderRunAway = false;

        public void UpdateEventState(MapEvent mapEvent)
        {
            bool newParty = false;
            MapEventSide attackerSide = mapEvent.AttackerSide;
            foreach (var party in attackerSide.Parties)
            {
                if (!PartyStates.ContainsKey(party.Id))
                {
                    newParty = true;
                    break;
                }
            }
            if (!newParty)
            {
                MapEventSide defenderSide = mapEvent.DefenderSide;
                foreach (var party in defenderSide.Parties)
                {
                    if (!PartyStates.ContainsKey(party.Id))
                    {
                        newParty = true;
                        break;
                    }
                }
            }
            if (newParty)
            {
                RegisterPartyAndTroops(mapEvent);
            }
            firstUpdated = true;
        }

        private void RegisterPartyAndTroops(MapEvent mapEvent)
        {
            // mapEvent.SimulateBattleSetup();
            MapEventSide attackerSide = mapEvent.AttackerSide;
            MapEventSide defenderSide = mapEvent.DefenderSide;

            for (int index = 0; index < attackerSide.NumRemainingSimulationTroops; index++)
            {
                UniqueTroopDescriptor troopDescriptor = MapEventSideHelper.SelectSimulationTroopAtIndex(attackerSide, index, out _);
                CharacterObject troop = attackerSide.GetAllocatedTroop(troopDescriptor);
                PartyBase troopParty = attackerSide.GetAllocatedTroopParty(troopDescriptor);

                var partyState = GetPartyState(troopParty);
                if (!partyState.Registered)
                    partyState.RegisterTroop(troop);
            }
            for (int index = 0; index < defenderSide.NumRemainingSimulationTroops; index++)
            {
                UniqueTroopDescriptor troopDescriptor = MapEventSideHelper.SelectSimulationTroopAtIndex(defenderSide, index, out _);
                CharacterObject troop = defenderSide.GetAllocatedTroop(troopDescriptor);
                PartyBase troopParty = defenderSide.GetAllocatedTroopParty(troopDescriptor);

                var partyState = GetPartyState(troopParty);
                if (!partyState.Registered)
                    partyState.RegisterTroop(troop);
            }

            foreach (var partyState in PartyStates.Values)
            {
                partyState.Registered = true;
            }
        }

        private PartyState GetPartyState(PartyBase party)
        {
            if (!PartyStates.TryGetValue(party.Id, out PartyState partyState))
            {
                PartyStates[party.Id] = new PartyState(this);
                return PartyStates[party.Id];
            }
            else
            {
                return partyState;
            }
        }

        public bool ApplyDamageToPartyTroop(AttackComposition attack, PartyBase party, CharacterObject troop, out float damage)
        {
            PartyState partyState = GetPartyState(party);
            return partyState.ApplyDamageToTroop(attack, troop, out damage);
        }

        public AttackComposition GetAttackPoints(PartyBase party, CharacterObject troop)
        {
            PartyState partyState = GetPartyState(party);
            return partyState.GetAttackPoints(troop);
        }

        public float GetTroopStrength(PartyBase party, CharacterObject troop)
        {
            PartyState partyState = GetPartyState(party);
            return partyState.GetTroopStrength(troop);
        }
    }
}