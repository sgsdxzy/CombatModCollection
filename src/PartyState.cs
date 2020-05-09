using System.Collections.Concurrent;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection
{
    public class PartyState
    {
        private readonly ConcurrentDictionary<MBGUID, TroopState> TroopStates = new ConcurrentDictionary<MBGUID, TroopState>();
        private readonly MapEventState mapEventState;

        public PartyState(MapEventState _mapEventState)
        {
            mapEventState = _mapEventState;
        }

        public TroopState GetTroopState(CharacterObject troop)
        {
            if (!TroopStates.TryGetValue(troop.Id, out TroopState troopState))
            {
                InformationManager.DisplayMessage(new InformationMessage("Warning: " + troop.Name.ToString() + " is not registered for battle."));
                TroopStates[troop.Id] = new TroopState(troop);
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
                troopState.PrepareWeapon(mapEventState);
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
                troopState.PrepareWeapon(mapEventState);
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

        public void RegisterTroops(CharacterObject troop, int count, bool isSiege = false)
        {
            TroopStates[troop.Id] = new TroopState(troop, isSiege);
            TroopStates[troop.Id].TotalCount += count;
        }
    }
}