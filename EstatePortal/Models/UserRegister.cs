using EstatePortal.Models;
using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public class UserRegister
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, EmailAddress, Compare("Email")]
        public string ConfirmEmail { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class EmployeeRegisterModel : UserRegister
    {
        public int EmployerId { get; set; } // Id of the inviting Employer
    }
}

