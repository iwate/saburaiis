using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SaburaIIS.Agent.Comparers
{
    public class WorkerProcessComparer : IEqualityComparer<POCO.WorkerProcess>, IKeySelector
    {
        public bool Equals(POCO.WorkerProcess? x, POCO.WorkerProcess? y)
        {
            return KeySelector(x!).Equals(KeySelector(y!));
        }

        public int GetHashCode([DisallowNull] POCO.WorkerProcess obj)
        {
            return KeySelector(obj).GetHashCode();
        }

        public Func<object, object> KeySelector { get; } = obj => ((POCO.WorkerProcess)obj).ProcessId;
    }
}