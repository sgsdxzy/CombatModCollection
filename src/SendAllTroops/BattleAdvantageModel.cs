using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CombatModCollection.SendAllTroops
{
    class BattleAdvantageModel
    {
        public static float PartyBattleAdvantage(PartyBase party)
        {
            float num = 1f;
            if (party.LeaderHero != null)
            {
                int skillValue = party.LeaderHero.GetSkillValue(DefaultSkills.Tactics);
                num += (float)((double)DefaultSkillEffects.TacticsAdvantage.PrimaryBonus * (double)skillValue * 0.01);
            }
            return num;
        }

        public static float GetSettlementAdvantage(Settlement settlement)
        {
            if (settlement.SiegeEvent != null && settlement.IsFortification)
            {
                int wallLevel = settlement.Town.GetWallLevel();
                bool flag = false;
                int num1 = 0;
                int num2 = 0;
                int num3 = 0;
                foreach (SiegeEvent.SiegeEngineConstructionProgress allSiegeEngine in settlement.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.AllSiegeEngines())
                {
                    if (allSiegeEngine.IsConstructed)
                    {
                        if (allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.Ram)
                            flag = true;
                        else if (allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.SiegeTower)
                            ++num1;
                        else if (allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.Trebuchet || allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.Onager || allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.Ballista)
                            ++num2;
                        else if (allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.FireOnager || allSiegeEngine.SiegeEngine == DefaultSiegeEngineTypes.FireBallista)
                            ++num3;
                    }
                }
                float num4 = (float)(2.0 + (double)(wallLevel - 1) * 0.5);
                if ((double)settlement.SettlementTotalWallHitPoints < 9.99999974737875E-06)
                    num4 *= 0.25f;
                return (float)((1.0 + (double)num4) / Math.Sqrt(1.0 + (flag | num1 > 0 ? 0.300000011920929 : 0.0) + (flag ? 0.4 : 0.0) + (num1 > 1 ? 0.449999988079071 : (num1 == 1 ? 0.300000011920929 : 0.0)) + (double)num2 * 0.200000002980232 + (double)num3 * 0.300000011920929));
            }
            return settlement.IsVillage ? 1.25f : 1f;
        }
    }
}
