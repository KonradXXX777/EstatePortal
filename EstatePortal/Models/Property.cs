using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tytuł")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Cena")]
        [Range(1, double.MaxValue, ErrorMessage = "Cena musi być liczbą dodatnią.")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Typ")]
        public PropertyType Type { get; set; } // Enum już utworzony wcześniej

        [Required]
        [Display(Name = "Powierzchnia")]
        [Range(1, double.MaxValue, ErrorMessage = "Cena musi być liczbą dodatnią.")]
        public decimal Area { get; set; }

        [Display(Name = "Cena za m²")]
        public decimal PricePerSquareMeter => Price / (Area > 0 ? Area : 1);

        [Display(Name = "Zdjęcia")]
        public List<string> MultimediaUrls { get; set; } = new List<string>();

        [Display(Name = "Link do filmu YouTube")]
        public string YouTubeVideoUrl { get; set; }

        // Lokalizacja
        [Display(Name = "Miejscowość")]
        public string City { get; set; }

        [Display(Name = "Dzielnica")]
        public string District { get; set; }

        [Display(Name = "Ulica/Osiedle")]
        public string Street { get; set; }

        // Dodatkowe informacje
        [Display(Name = "Opis")]
        public string Description { get; set; }
    }
}
