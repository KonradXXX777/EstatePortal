using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public enum SecurityType
    {
        [Display(Name = "Monitoring")]
        Monitoring,
        [Display(Name = "Ochrona")]
        Security
    }

    public enum NeighborhoodType
    {

        [Display(Name = "Miejska")]
        Urban,
        [Display(Name = "Podmiejska")]
        Suburban,
        [Display(Name = "Wiejska")]
        Rural
    }
}
