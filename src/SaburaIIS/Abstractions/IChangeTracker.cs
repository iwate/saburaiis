using System;
using System.Threading.Tasks;

namespace SaburaIIS
{
    public interface IChangeTracker : IDisposable
    {
        Task<bool> HasChangeAsync();
    }
}
