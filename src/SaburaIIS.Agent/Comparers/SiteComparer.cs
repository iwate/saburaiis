using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SaburaIIS.Agent.Comparers
{
    public class SiteComparer : IEqualityComparer<POCO.Site>, IKeySelector
    {
        public bool Equals(POCO.Site? x, POCO.Site? y)
        {
            return KeySelector(x!).Equals(KeySelector(y!));
        }

        public int GetHashCode([DisallowNull] POCO.Site obj)
        {
            return KeySelector(obj).GetHashCode();
        }

        public Func<object, object> KeySelector { get; } = obj => ((POCO.Site)obj).Name;
    }
}