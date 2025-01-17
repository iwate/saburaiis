using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using SaburaIIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaburaIIS.Azure
{
    public class AzKeyVault : IVault
    {
        private readonly CertificateClient _certClient;
        private readonly SecretClient _secretClient;
        public AzKeyVault(Config config)
        {
            _certClient = CreateCertificateClient(config);
            _secretClient = CreateSecretClient(config);
        }

        public virtual async Task<IEnumerable<Certificate>> GetCertificatesAsync()
        {
            var result = new List<Certificate>();
            var now = DateTimeOffset.UtcNow;
            await foreach(var certPropsPage in _certClient.GetPropertiesOfCertificatesAsync().AsPages())
            {
                foreach (var certProp in certPropsPage.Values.Where(cert => (!cert.ExpiresOn.HasValue || cert.ExpiresOn.Value > now) && cert.Enabled != false))
                {
                    await foreach(var verPropPage in _certClient.GetPropertiesOfCertificateVersionsAsync(certProp.Name).AsPages())
                    {
                        foreach (var verProp in verPropPage.Values.Where(ver => (!ver.ExpiresOn.HasValue || ver.ExpiresOn.Value > now) && ver.Enabled != false))
                        {
                            result.Add(new Certificate
                            {
                                Name = certProp.Name,
                                Version = verProp.Version,
                                NotBefore = verProp.NotBefore,
                                ExpiresOn = verProp.ExpiresOn,
                                Thumbprint = verProp.X509Thumbprint
                            });
                        }
                    }
                }
            }
            return result;
        }

        public virtual async Task<byte[]> GetCertificateAsync(string name, string version)
        {
            var cert = await _certClient.GetCertificateVersionAsync(name, version);
            string[] segments = cert.Value.SecretId.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length != 3)
            {
                throw new InvalidOperationException($"Number of segments is incorrect: {segments.Length}, URI: {cert.Value.SecretId}");
            }
            string secretName = segments[1];
            string secretVersion = segments[2];

            var secret = await _secretClient.GetSecretAsync(secretName, secretVersion);

            return Convert.FromBase64String(secret.Value.Value);
        }

        private static CertificateClient CreateCertificateClient(Config config)
        {
            if (!string.IsNullOrEmpty(config.AADClientSecret))
            {
                var servicePrincipal = new ClientSecretCredential(config.AADTenantId, config.AADClientId, config.AADClientSecret);
                return new CertificateClient(new Uri(config.GetKeyVaultEndpoint()), servicePrincipal);
            }

            return new CertificateClient(new Uri(config.GetKeyVaultEndpoint()), new DefaultAzureCredential());
        }

        private static SecretClient CreateSecretClient(Config config)
        {
            if (!string.IsNullOrEmpty(config.AADClientSecret))
            {
                var servicePrincipal = new ClientSecretCredential(config.AADTenantId, config.AADClientId, config.AADClientSecret);
                return new SecretClient(new Uri(config.GetKeyVaultEndpoint()), servicePrincipal);
            }

            return new SecretClient(new Uri(config.GetKeyVaultEndpoint()), new DefaultAzureCredential());
        }
    }
}
