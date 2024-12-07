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
		public float Area { get; set; }

		[Required]
		public decimal Price { get; set; }

		[Required]
		public string Location { get; set; } = string.Empty;

		public string Street { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

		[Required]
		public SaleOrRent SaleOrRent { get; set; }

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
    }
}
