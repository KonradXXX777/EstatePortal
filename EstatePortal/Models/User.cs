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
        public byte[] PassowrdHash { get; set; } = new byte[64];

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public bool AcceptTerms { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string CompanyName { get; set; }

        public string NIP { get; set; }

        public string Address { get; set; }

        public DateTime DateRegistered { get; set; }

        public int? EmployerId { get; set; }

        [ForeignKey("EmployerId")]
        public virtual User Employer { get; set; }

        public virtual ICollection<User> Employees { get; set; } = new List<User>();
    }
}
