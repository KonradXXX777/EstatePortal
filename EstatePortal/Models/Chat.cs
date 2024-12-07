using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        [Required]
        public int AnnouncementId { get; set; }

        [ForeignKey("AnnouncementId")]
        public virtual Announcement Announcement { get; set; }

        public DateTime LastMessageTime { get; set; }
        public string LastMessage { get; set; } = string.Empty;

        public User Buyer { get; set; }
        public User Seller { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
