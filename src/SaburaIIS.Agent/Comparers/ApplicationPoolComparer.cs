using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SaburaIIS.Agent.Comparers
{
    public class ApplicationPoolComparer : IEqualityComparer<POCO.ApplicationPool>, IKeySelector
    {
        public bool Equals(POCO.ApplicationPool? x, POCO.ApplicationPool? y)
        {
            return KeySelector(x!).Equals(KeySelector(y!));
        }

        public int GetHashCode([DisallowNull] POCO.ApplicationPool obj)
        {
            return KeySelector(obj).GetHashCode();
        }

        public Func<object, object> KeySelector { get; } = obj => ((POCO.ApplicationPool)obj).Name;
    }
}