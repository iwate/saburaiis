using System;

namespace SaburaIIS.Models
{
    public class Release
    {
        public string Version { get; set; }
        public string Url { get; set; }
        public DateTimeOffset ReleaseAt { get; set; }
    }
}
