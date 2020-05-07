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
        public bool Registered = false;

        public PartyState(MapEventState _mapEventState)
        {
            mapEventState = _mapEventState;
        }

        public TroopState GetTroopState(CharacterObject troop)
        {
            if (!TroopStates.TryGetValue(troop.Id, out TroopState troopState))
            {
                InformationManager.DisplayMessage(new InformationMessage(troop.Name.ToString() + " not registered "));
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

        public AttackComposition GetAttackPoints(CharacterObject troop)
        {
            if (SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                TroopState troopState = GetTroopState(troop);
                troopState.PrepareWeapon(mapEventState);
                return troopState.DoAttack();
            }
            else
            {
                return new AttackComposition
                {
                    Melee = troop.GetPower()
                };
            }
        }

        public float GetTroopStrength(CharacterObject troop)
        {
            TroopState troopState = GetTroopState(troop);
            return troopState.Hitpoints / troop.MaxHitPoints() * troop.GetPower();
        }

        public void RegisterTroop(CharacterObject troop)
        {
            if (!TroopStates.TryGetValue(troop.Id, out TroopState troopState))
            {
                TroopStates[troop.Id] = new TroopState(troop);
            }
            else
            {
                TroopStates[troop.Id].Count += 1;
            }
        }
    }
}