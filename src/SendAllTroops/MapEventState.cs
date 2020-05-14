using CombatModCollection.SendAllTroops;
using System;
using System.Collections.Concurrent;
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
                mapEventState = new MapEventState(mapEvent);
                AllMapEventStates[mapEvent.Id] = mapEventState;
            }
            return mapEventState;
        }

        public static void RemoveMapEventState(MapEvent mapEvent)
        {
            AllMapEventStates.TryRemove(mapEvent.Id, out _);
        }


        private readonly ConcurrentDictionary<string, PartyState> PartyStates = new ConcurrentDictionary<string, PartyState>();
        public bool IsSiege = false;
        // Only when DCM is off, when DCM is on, the penalty is applied in TroopState.GetWeaponDamage
        public float SettlementPenalty = 1;

        // For DetailedCombatModel
        public int BattleScale;
        public int StageRounds;
        public readonly int WallLevel;
        public readonly float MeleePenaltyForAttacker;
        private readonly int NumberOfRoundsBeforeGateBreach;
        public bool GateBreached { get { return StageRounds > NumberOfRoundsBeforeGateBreach; } }

        // For LearnToQuit
        public bool IsDefenderRunAway = false;

        private MapEventState(MapEvent mapEvent)
        {
            if (Settings.Instance.Battle_SendAllTroops)
            {
                StageRounds = (int)MapEvent__mapEventUpdateCount.GetValue(mapEvent);
                IsSiege = mapEvent.IsSiegeAssault;
                if (Settings.Instance.Battle_SendAllTroops_DetailedCombatModel)
                {
                    if (IsSiege)
                    {
                        BattleScale = 4;
                        WallLevel = mapEvent.MapEventSettlement.Town.GetWallLevel();

                        int Ram = 0;
                        int SiegeTower = 0;
                        int Other = 0;
                        foreach (SiegeEvent.SiegeEngineConstructionProgress allSiegeEngine in mapEvent.MapEventSettlement.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.AllSiegeEngines())
                        {
                            if (allSiegeEngine.IsConstructed)
                            {
                                if (allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.Ram)
                                    Ram += 1;
                                else if (allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.SiegeTower)
                                    SiegeTower += 1;
                                else if (allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.Trebuchet || allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.Onager || allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.Ballista)
                                    Other += 1;
                                else if (allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.FireOnager || allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.FireBallista)
                                    Other += 1;
                            }
                        }
                        if (mapEvent.MapEventSettlement.SettlementTotalWallHitPoints < 1e-5)
                        {
                            NumberOfRoundsBeforeGateBreach = 5 + BattleScale;
                            MeleePenaltyForAttacker = WallLevel * 0.15f - SiegeTower * 0.05f;
                        }
                        else
                        {
                            NumberOfRoundsBeforeGateBreach = Math.Max(WallLevel * 10 - Ram * 15 - SiegeTower * 5 - Other * 3, 0) + 10 + BattleScale;
                            MeleePenaltyForAttacker = WallLevel * 0.3f - SiegeTower * 0.10f;
                        }
                    }
                    else
                    {
                        if (mapEvent.GetNumberOfInvolvedMen() > 100)
                        {
                            BattleScale = 3;
                        }
                        else
                        {
                            BattleScale = mapEvent.GetNumberOfInvolvedMen() > 50 ? 2 : 1;
                        }
                    }
                }
                else
                {
                    if (IsSiege)
                    {
                        SettlementPenalty = 1.0f / BattleAdvantageModel.GetSettlementAdvantage(mapEvent.MapEventSettlement);
                    }
                }
            }
        }

        private PartyState GetPartyState(PartyBase party)
        {
            if (!PartyStates.TryGetValue(party.Id, out PartyState partyState))
            {
                partyState = new PartyState(this, party.Side == BattleSideEnum.Attacker);
                PartyStates[party.Id] = partyState;
                for (int index = 0; index < party.MemberRoster.Count; ++index)
                {
                    TroopRosterElement elementCopyAtIndex = party.MemberRoster.GetElementCopyAtIndex(index);
                    CharacterObject troop = elementCopyAtIndex.Character;
                    if (troop != null)
                    {
                        partyState.RegisterTroops(troop, (elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber));
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

        public AttackComposition MakePartyAttack(PartyBase party, float consumption)
        {
            PartyState partyState = GetPartyState(party);
            return partyState.MakePartyAttack(consumption);
        }

        public bool ApplyDamageToPartyTroop(AttackComposition attack, PartyBase party, CharacterObject troop, out float damage)
        {
            PartyState partyState = GetPartyState(party);
            return partyState.ApplyDamageToTroop(attack, troop, out damage);
        }
    }
}