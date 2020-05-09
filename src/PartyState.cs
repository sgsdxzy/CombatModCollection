using System.Collections.Concurrent;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection
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
                TroopStates[troop.Id] = new TroopState(this, troop, 1);
                return TroopStates[troop.Id];
            }
            else
            {
                return troopState;
            }
        }

        public bool ApplyDamageToTroop(AttackComposition attack, CharacterObject troop, out float damage)
        {
            TroopState troopState = GetTroopState(troop);
            bool isFinishingBlow = troopState.TakeHit(attack, out damage);
            return isFinishingBlow;
        }

        public AttackComposition GetAttack(CharacterObject troop)
        {
            if (SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                TroopState troopState = GetTroopState(troop);
                troopState.PrepareWeapon();
                return troopState.DoSingleAttack();
            }
            else
            {
                return new AttackComposition
                {
                    Melee = troop.GetPower()
                };
            }
        }

        public AttackComposition GetPartyAttack()
        {
            AttackComposition attack = new AttackComposition();
            foreach (var troopState in TroopStates.Values)
            {
                troopState.PrepareWeapon();
                attack += troopState.DoTotalAttack();
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