using System;

namespace SaburaIIS.POCO
{
    public partial class ApplicationPool
    {
        public DateTimeOffset RecycleRequestAt { get; set; }
    }
}
