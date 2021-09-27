using SaburaIIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SaburaIIS.Agent.Extensions
{
    public static class PartitionExtensions
    {
        public static IEnumerable<IGrouping<string, (string thumbprint, string store)>> GetCertificates(this Partition partition)
            => partition.Sites.SelectMany(site =>
                                site.Bindings
                                    .Where(binding => binding.CertificateHash != null)
                                    .Select(binding => (Convert.ToHexString(binding.CertificateHash), binding.CertificateStoreName)))
                            .Distinct()
                            .GroupBy(cert => cert.CertificateStoreName);
    }
}
