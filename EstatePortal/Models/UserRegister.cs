using EstatePortal.Models;
using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public class UserRegister
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), MinLength(8)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare("PasswordHash")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public bool AcceptTerms { get; set; }
    }

    public class EstateAgencyRegister : UserRegister
    {
        public string CompanyName { get; set; } = string.Empty;
        public string NIP { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class DeveloperRegister : EstateAgencyRegister
    {
    }

    public class EmployeeRegister : UserRegister
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        //Invitation Token
        public string InvitationToken { get; set; } = string.Empty;
    }
}

