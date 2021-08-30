using System.ComponentModel.DataAnnotations;

namespace SaburaIIS.AdminWeb.ViewModels
{
    public class PackageCreationViewModel
    {
        [Required]
        [RegularExpression(Validations.RegularExpression.PackageName)]
        public string Name { get; set; }
    }
}
