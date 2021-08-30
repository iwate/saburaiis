using System;
using System.Collections.Generic;

namespace SaburaIIS.Agent.Comparers
{
    public interface IKeySelector
    {
        Func<object, object> KeySelector { get; }
    }
}
