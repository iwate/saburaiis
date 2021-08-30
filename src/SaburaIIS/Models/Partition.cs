using SaburaIIS.POCO;
using System.Collections.Generic;

namespace SaburaIIS.Models
{
    public class Partition
    {
        public string id { get; set; }
        public string Name
        {
            get => id;
            set => id = value;
        }
        public ICollection<ApplicationPool> ApplicationPools { get; set; } = new List<ApplicationPool>();
        public ICollection<Site> Sites { get; set; } = new List<Site>();
        public ICollection<VirtualMachineScaleSet> ScaleSets { get; set; } = new List<VirtualMachineScaleSet>();
    }
}
