using System;

namespace SaburaIIS.Models
{
    public class Certificate
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public byte[] Thumbprint { get; set; }
        public DateTimeOffset? NotBefore { get; set; }
        public DateTimeOffset? ExpiresOn { get; set; }
    }
}
