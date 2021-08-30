using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SaburaIIS.Agent.Comparers
{
    public class VirtualDirectoryComparer : IEqualityComparer<POCO.VirtualDirectory>, IKeySelector
    {
        public bool Equals(POCO.VirtualDirectory? x, POCO.VirtualDirectory? y)
        {
            return KeySelector(x!).Equals(KeySelector(y!));
        }

        public int GetHashCode([DisallowNull] POCO.VirtualDirectory obj)
        {
            return KeySelector(obj).GetHashCode();
        }

        public Func<object, object> KeySelector { get; } = obj => ((POCO.VirtualDirectory)obj).Path;
    }
}