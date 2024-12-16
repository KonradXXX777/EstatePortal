using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePortal.Models
{
	public class AnnouncementPhoto
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Url { get; set; } = string.Empty; // Adres URL zdjęcia

		[ForeignKey("AnnouncementId")]
		public int AnnouncementId { get; set; }
		public virtual Announcement Announcement { get; set; } // Powiązanie z ogłoszeniem
	}
}
