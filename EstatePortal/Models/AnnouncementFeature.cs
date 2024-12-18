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

        // Bezpieczeństwo
        public SecurityType? Security { get; set; }

        // Dostosowanie dla osób niepełnosprawnych
        public bool IsAccessible { get; set; }

        // Zabudowa w okolicy
        public NeighborhoodType? Neighborhood { get; set; } 

        // Przyroda w okolicy
        public bool HasPark { get; set; }
        public bool HasForest { get; set; }
        public bool HasLake { get; set; }
        public bool HasMountains { get; set; }
        public bool HasSea { get; set; }

        // Dodatkowe informacje
		public bool HasGarden { get; set; } // Ogród
        public bool HasBasement { get; set; } // Piwnica
        public bool HasAttic { get; set; } // Strych
        public bool HasGarage { get; set; } // Garaż
        public bool HasAirConditioning { get; set; } // Klimatyzacja
        public bool HasPool { get; set; } // Basen

        // Media
        public bool HasWater { get; set; }
        public bool HasElectricity { get; set; }
        public bool HasGas { get; set; }
        public bool HasSewerage { get; set; } // Kanalizacja
        public bool HasInternet { get; set; }

        public bool Garden { get; set; } // Ogródek
		public bool KitchenAppliances { get; set; } // Wyposażenie AGD
		public bool Furnished { get; set; } // Umeblowanie

		public string? SafetyFeatures { get; set; } // Bezpieczeństwo

		public string? Surroundings { get; set; } // Zabudowa w okolicy
		public string? Nature { get; set; } // Przyroda w okolicy (np. park, las, morze, góry)

		[ForeignKey("AnnouncementId")]
		public int AnnouncementId { get; set; }
		public virtual Announcement Announcement { get; set; } // Powiązanie z ogłoszeniem
	}
}
