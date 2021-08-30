using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SaburaIIS.Agent.Comparers
{
    public class CustomLogFieldComparer : IEqualityComparer<POCO.CustomLogField>, IKeySelector
    {
        public bool Equals(POCO.CustomLogField? x, POCO.CustomLogField? y)
        {
            return KeySelector(x!).Equals(KeySelector(y!));
        }

        public int GetHashCode([DisallowNull] POCO.CustomLogField obj)
        {
            return KeySelector(obj).GetHashCode();
        }

        public Func<object, object> KeySelector { get; } = obj => ((POCO.CustomLogField)obj).LogFieldName;
    }
}