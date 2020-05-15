using System.Collections.Concurrent;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection.LearnToQuit.MapEventPatches
{
    internal class MapEventCustomMembers
    {
        public static readonly ConcurrentDictionary<MBGUID, bool> DefendersRanAway = new ConcurrentDictionary<MBGUID, bool>();
    }
}
