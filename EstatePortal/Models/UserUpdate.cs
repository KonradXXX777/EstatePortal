using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public class UserUpdate
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail.")]

        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password), MinLength(8)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Nowe hasło i potwierdzenie hasła muszą być takie same.")]
        public string ConfirmPassword { get; set; }
    }
}
