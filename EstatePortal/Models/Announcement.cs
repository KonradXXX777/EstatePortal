using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePortal.Models
{
	public class Announcement
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Title { get; set; } = string.Empty;

		[Required]
		public double Area { get; set; } // Powierzchnia użytkowa w m²

		[Required]
		public decimal Price { get; set; } // Cena

		public decimal? PricePerSquareMeter { get; set; } // Cena za m² (opcjonalna)

		[Required]
		public string Location { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;

		[Required]
		public SaleOrRent SaleOrRent { get; set; } // Na sprzedaż lub wynajem

		[Required]
		public PropertyType PropertyType { get; set; } // Typ nieruchomości

		[Required]
		public AnnouncementStatus Status { get; set; } = AnnouncementStatus.PendingApproval; // Status ogłoszenia

		public DateTime DateCreated { get; set; } = DateTime.UtcNow; // Data dodania ogłoszenia
		public DateTime? DateUpdated { get; set; } // Data ostatniej aktualizacji ogłoszenia

		public string? VideoUrls { get; set; } // Adresy URL do filmów (np. YouTube), przechowywane jako JSON

		public virtual AnnouncementFeature Features { get; set; } = new AnnouncementFeature(); // Cechy ogłoszenia
		public virtual ICollection<AnnouncementPhoto> Photos { get; set; } = new List<AnnouncementPhoto>(); // Zdjęcia

		[ForeignKey("UserId")]
		public int UserId { get; set; }
		public virtual User User { get; set; } // Powiązanie z użytkownikiem
	}
}
