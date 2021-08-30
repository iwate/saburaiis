using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SaburaIIS.Agent.Comparers
{
    public class ScheduleComparer : IEqualityComparer<POCO.Schedule>, IKeySelector
    {
        public bool Equals(POCO.Schedule? x, POCO.Schedule? y)
        {
            return KeySelector(x!).Equals(KeySelector(y!));
        }

        public int GetHashCode([DisallowNull] POCO.Schedule obj)
        {
            return KeySelector(obj).GetHashCode();
        }

        public Func<object, object> KeySelector { get; } = obj => ((POCO.Schedule)obj).Time;
    }
}