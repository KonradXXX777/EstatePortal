using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePortal.Models
{
    public class EmployeeInvitation
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; // Adres e-mail zaproszonego pracownika

        [Required]
        public string Token { get; set; } = Guid.NewGuid().ToString(); // Unikalny token zaproszenia

        [Required]
        public int EmployerId { get; set; } // Id pracodawcy, który wysyła zaproszenie

        [ForeignKey("EmployerId")]
        public virtual User Employer { get; set; } // Relacja z pracodawcą

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data utworzenia zaproszenia
        public DateTime? ExpiryDate { get; set; } // Opcjonalnie: data wygaśnięcia tokena
    }
}
