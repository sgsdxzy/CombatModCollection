using System.Collections.Concurrent;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection.SendAllTroops
{
    public class PartyState
    {
        private readonly ConcurrentDictionary<MBGUID, TroopState> TroopStates = new ConcurrentDictionary<MBGUID, TroopState>();
        public readonly MapEventState mapEventState;
        public readonly bool IsAttacker;

        public PartyState(MapEventState _mapEventState, bool _IsAttacker)
        {
            mapEventState = _mapEventState;
            IsAttacker = _IsAttacker;
        }

        public TroopState GetTroopState(CharacterObject troop)
        {
            if (!TroopStates.TryGetValue(troop.Id, out TroopState troopState))
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Warning: " + troop.Name.ToString() + " is not registered for battle."));
                troopState = new TroopState(this, troop, 1);
                TroopStates[troop.Id] = troopState;
            }
            return troopState;
        }

        public bool ApplyDamageToTroop(PartyAttackComposition attack, CharacterObject troop, out float damage, out int heroRemainingHP)
        {
            TroopState troopState = GetTroopState(troop);
            bool isFinishingBlow = troopState.TakeHit(attack, out damage, out heroRemainingHP);
            return isFinishingBlow;
        }

        public PartyAttackComposition MakePartyAttack(float consumption)
        {
            PartyAttackComposition attack = new PartyAttackComposition();
            foreach (var troopState in TroopStates.Values)
            {
                attack += troopState.MakeTotalAttack(consumption);
            }
            return attack;
        }

        public float GetCurrentStrength()
        {
            float totalStrength = 0;
            foreach (var troopState in TroopStates.Values)
            {
                totalStrength += troopState.GetCurrentStrength();
            }
            return totalStrength;
        }

        public void RegisterTroops(CharacterObject troop, int count)
        {
            TroopStates[troop.Id] = new TroopState(this, troop, count);
        }
    }
}