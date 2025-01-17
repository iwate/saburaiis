using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaburaIIS
{
    public interface IVariables
    {
        Task<IEnumerable<KeyValuePair<string, string>>> GetVariables(string partitionName, string applicationPoolName);
    }
}
