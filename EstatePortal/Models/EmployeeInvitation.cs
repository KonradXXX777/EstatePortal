using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePortal.Models
{
    public class EmployeeInvitation
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; 

        [Required]
        public string Token { get; set; } = Guid.NewGuid().ToString(); 

        [Required]
        public int EmployerId { get; set; } 

        [ForeignKey("EmployerId")]
        public virtual User Employer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
    }
}
