using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }
        public DateTime DateRegistered { get; set; } 
    }

    public class EstateAgency : User
    {
        public string AgencyName { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }

    public class Developer : User
    {
        public string DeveloperName { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }

    public class Employee : User
    {
        public int EmployerId { get; set; } // Foreign key to either EstateAgency or Developer
        public User Employer { get; set; }
    }
}

