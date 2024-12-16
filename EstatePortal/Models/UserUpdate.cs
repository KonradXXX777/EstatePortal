using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public class UserUpdate
    {
<<<<<<< HEAD
<<<<<<< HEAD
        [Required(ErrorMessage = "Imię jest wymagane.")]
=======
>>>>>>> register
        public string FirstName { get; set; }

        public string LastName { get; set; }
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail.")]

        public string Email { get; set; }

        [DataType(DataType.Password)]
=======
		public UserRole Role { get; set; } // Informacja o roli użytkownika

		//[Required(ErrorMessage = "Imię jest wymagane.")]
		public string FirstName { get; set; }

        //[Required(ErrorMessage = "Nazwisko jest wymagane.")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail.")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
		public string CompanyName { get; set; }
		public string NIP { get; set; }
		public string Address { get; set; }

		[DataType(DataType.Password)]
>>>>>>> master
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password), MinLength(8)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Nowe hasło i potwierdzenie hasła muszą być takie same.")]
        public string ConfirmPassword { get; set; }
    }
}
