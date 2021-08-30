using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SaburaIIS.Agent.Comparers
{
    public class BindingComparer : IEqualityComparer<POCO.Binding>, IKeySelector
    {
        public bool Equals(POCO.Binding? x, POCO.Binding? y)
        {
            return KeySelector(x!).Equals(KeySelector(y!));
        }

        public int GetHashCode([DisallowNull] POCO.Binding obj)
        {
            return KeySelector(obj).GetHashCode();
        }

        public Func<object, object> KeySelector { get; } = obj => ((POCO.Binding)obj).BindingInformation;
    }
}