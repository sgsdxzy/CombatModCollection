using System.Collections.Concurrent;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection
{
    public class TroopStat
    {
        public float Hitpoints;
    }

    public class MapEventStat
    {
        public ConcurrentDictionary<MBGUID, TroopStat> TroopStats = new ConcurrentDictionary<MBGUID, TroopStat>();
        public int StageRounds = 0;
    }

    public class GlobalStorage
    {
        public static ConcurrentDictionary<MBGUID, bool> IsDefenderRunAway = new ConcurrentDictionary<MBGUID, bool>();
        public static ConcurrentDictionary<MBGUID, MapEventStat> MapEventStats = new ConcurrentDictionary<MBGUID, MapEventStat>();
    }
}