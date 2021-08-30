using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SaburaIIS.Agent.Comparers
{
    public class ApplicationComparer : IEqualityComparer<POCO.Application>, IKeySelector
    {
        public bool Equals(POCO.Application? x, POCO.Application? y)
        {
            return KeySelector(x!).Equals(KeySelector(y!));
        }

        public int GetHashCode([DisallowNull] POCO.Application obj)
        {
            return KeySelector(obj).GetHashCode();
        }

        public Func<object, object> KeySelector { get; } = new Func<object, object>(obj => ((POCO.Application)obj).Path);
    }
}