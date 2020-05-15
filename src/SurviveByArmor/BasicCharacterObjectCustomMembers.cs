using System.Collections.Concurrent;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection.SurviveByArmor
{
    class BasicCharacterObjectCustomMembers
    {
        public static readonly ConcurrentDictionary<MBGUID, float> ExcessiveDamages = new ConcurrentDictionary<MBGUID, float>();
    }
}
