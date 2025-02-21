using SaburaIIS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaburaIIS
{
    public interface IVault
    {
        Task<IEnumerable<Certificate>> GetCertificatesAsync();
        Task<byte[]> GetCertificateAsync(string name, string version);
    }
}
