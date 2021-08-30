using System.Collections.Generic;

namespace SaburaIIS.AdminWeb.ViewModels
{
    public class PackagesSummaryViewModel
    {
        public string LocationRoot { get; set; }
        public IEnumerable<string> Packages { get; set; }
    }
}
