using SaburaIIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SaburaIIS.Agent.Extensions
{
    public static class CertificateExtensions
    {
        public static Certificate Find(this IEnumerable<Certificate> certificates, string thumbprint)
            => certificates.First(o => Convert.ToHexString(o.Thumbprint) == thumbprint);
    }
}
