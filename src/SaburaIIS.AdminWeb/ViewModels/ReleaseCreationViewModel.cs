using System;
using System.ComponentModel.DataAnnotations;

namespace SaburaIIS.AdminWeb.ViewModels
{
    public class ReleaseCreationViewModel
    {
        [Required]
        [RegularExpression(Validations.RegularExpression.ReleaseVersion)]
        public string Version { get; set; }
        [Required]
        [RegularExpression(Validations.RegularExpression.ReleaseUrl)]
        public string Url { get; set; }
    }
}
