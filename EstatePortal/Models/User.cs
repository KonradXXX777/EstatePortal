using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public byte[] PasswordHash { get; set; } = new byte[64];

        [Required]
        public byte[] PasswordSalt { get; set; }

        [Required]
        public UserRole Role { get; set; }
<<<<<<< HEAD

        [Required]
        public bool AcceptTerms { get; set; }

        public string? FirstName { get; set; }
=======
        [Required]
        public UserStatus Status { get; set; } = UserStatus.Active;

        [Required(ErrorMessage = "Musisz zaakceptować regulamin.")]
		public bool AcceptTerms { get; set; } = false;

		public string? FirstName { get; set; }
>>>>>>> master

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? CompanyName { get; set; }

        public string? NIP { get; set; }

        public string? Address { get; set; }

        // Registration Email Verification
        public DateTime DateRegistered { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }

        // Password Reset (button "Zapomnialem hasla)
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public DateTime? PasswordLastReset { get; set; }

        public int? EmployerId { get; set; }

        [ForeignKey("EmployerId")]
        public virtual User Employer { get; set; }

        //public virtual ICollection<User> Employees { get; set; } = new List<User>();
        public virtual ICollection<EmployeeInvitation> EmployeeInvitations { get; set; } = new List<EmployeeInvitation>();


    }
}
