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
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Area { get; set; }

		[Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

		[Required]
		public string Location { get; set; } = string.Empty;

		public string Street { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

		//[Required]
		public SellOrRent SellOrRent { get; set; }

		[Required]
		public PropertyType PropertyType { get; set; } 

		[Required]
		public AnnouncementStatus Status { get; set; } = AnnouncementStatus.PendingApproval; 

		public DateTime DateCreated { get; set; } = DateTime.UtcNow; 
		public DateTime? DateUpdated { get; set; } 

		public string? VideoUrls { get; set; }

		public virtual AnnouncementFeature Features { get; set; } = new AnnouncementFeature();
		public virtual ICollection<AnnouncementPhoto> Photos { get; set; } = new List<AnnouncementPhoto>();

		[ForeignKey("UserId")]
		public int UserId { get; set; }
		public virtual User User { get; set; } 

        [NotMapped] // Do not save in db
        public string? MainPhotoUrl => Photos.FirstOrDefault()?.Url;

        public string SellOrRentDisplay
        {
            get
            {
                return SellOrRent switch
                {
                    SellOrRent.Sell => "Na sprzedaż",
                    SellOrRent.Rent => "Wynajem",
                    _ => "Nieznany"
                };
            }
        }
        public string PropertyTypeDisplay
        {
            get
            {
                return PropertyType switch
                {
                    PropertyType.Flat => "Mieszkanie",
                    PropertyType.House => "Dom",
                    PropertyType.Plot => "Działka",
                    PropertyType.Garage => "Garaż",
                    PropertyType.CommercialPremises => "Lokal użytkowy",
                    PropertyType.Warehouse => "Magazyn",
                    PropertyType.Room => "Pokój",
                    _ => "Nieznany"
                };
            }
        }
        public string AnnouncementStatusDisplay
        {
            get
            {
                return Status switch
                {
                    AnnouncementStatus.Active => "Aktywne",
                    AnnouncementStatus.Inactive => "Nieaktywne",
                    AnnouncementStatus.PendingApproval => "Oczekuje na zatwierdzenie",
                    AnnouncementStatus.Rejected => "Odrzucone",
                    AnnouncementStatus.Sold => "Sprzedane",
                    _ => "Nieznany"
                };
            }
        }
    }
}