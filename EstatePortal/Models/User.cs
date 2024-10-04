using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Lub użyj IdentityUser

        [Required]
        public UserRole Role { get; set; }

        public DateTime DateRegistered { get; set; }

        // Jeśli użytkownik jest pracownikiem, to wskazuje na pracodawcę
        public int? EmployerId { get; set; }

        [ForeignKey("EmployerId")]
        public virtual User Employer { get; set; }

        // Dla biur nieruchomości i deweloperów - lista pracowników
        public virtual ICollection<User> Employees { get; set; }

        // Dla biur nieruchomości i deweloperów
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public User()
        {
            Employees = new List<User>();
        }
    }
}

