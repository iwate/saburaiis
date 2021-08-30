using System;
using System.Collections.Generic;

namespace SaburaIIS.Models
{
    public class Package
    {
        public string id { get; set; }
        public string Name
        {
            get => id;
            set => id = value;
        }
        public ICollection<Release> Releases { get; set; } = new List<Release>();
    }
}
