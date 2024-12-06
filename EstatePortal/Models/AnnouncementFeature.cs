using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePortal.Models
{
	public class AnnouncementFeature
	{
		[Key]
		public int Id { get; set; }

		public int? Rooms { get; set; } // Liczba pokoi
		public int? Floor { get; set; } // Piętro
		public int? TotalFloors { get; set; } // Liczba pięter w budynku

		public bool Elevator { get; set; } // Winda
		public int? YearBuilt { get; set; } // Rok budowy

		public string? PropertyCondition { get; set; } // Stan nieruchomości
		public string? Parking { get; set; } // Miejsce parkingowe
		public string? EnergyCertificate { get; set; } // Świadectwo energetyczne

		public bool Garden { get; set; } // Ogródek
		public bool KitchenAppliances { get; set; } // Wyposażenie AGD
		public bool Furnished { get; set; } // Umeblowanie

		public bool HasWater { get; set; } // Media: Woda
		public bool HasElectricity { get; set; } // Media: Prąd
		public bool HasGas { get; set; } // Media: Gaz
		public bool HasSewerage { get; set; } // Media: Kanalizacja
		public bool HasInternet { get; set; } // Media: Internet

		public string? SafetyFeatures { get; set; } // Bezpieczeństwo
		public bool IsDisabledAccessible { get; set; } // Dostosowane dla osób niepełnosprawnych

		public string? Surroundings { get; set; } // Zabudowa w okolicy
		public string? Nature { get; set; } // Przyroda w okolicy (np. park, las, morze, góry)

		public bool HasBasement { get; set; } // Piwnica
		public bool HasAttic { get; set; } // Strych
		public bool HasGarage { get; set; } // Garaż
		public bool HasAirConditioning { get; set; } // Klimatyzacja
		public bool HasPool { get; set; } // Basen

		[ForeignKey("AnnouncementId")]
		public int AnnouncementId { get; set; }
		public virtual Announcement Announcement { get; set; } // Powiązanie z ogłoszeniem
	}
}
