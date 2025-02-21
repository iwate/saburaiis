using SaburaIIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SaburaIIS.Local
{
    public class MachineVault : IVault
    {
        public MachineVault(Config config)
        {
        }

        public Task<byte[]> GetCertificateAsync(string name, string version)
        {
            using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

            store.Open(OpenFlags.ReadOnly);

            var certs = store.Certificates.Find(X509FindType.FindBySerialNumber, version, false);

            return Task.FromResult(certs.FirstOrDefault()?.RawData);
        }

        public Task<IEnumerable<Certificate>> GetCertificatesAsync()
        {
            using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

            store.Open(OpenFlags.ReadOnly);

            return Task.FromResult(store.Certificates.Select(c => new Certificate
            {
                Name = c.Subject,
                Version = c.SerialNumber,
                Thumbprint = Convert.FromHexString(c.Thumbprint),
                ExpiresOn = c.NotAfter,
                NotBefore = c.NotBefore,
            }));
        }
    }
}
