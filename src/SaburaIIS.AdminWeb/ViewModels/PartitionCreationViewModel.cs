using System.ComponentModel.DataAnnotations;

namespace SaburaIIS.AdminWeb.ViewModels
{
    public class PartitionCreationViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
