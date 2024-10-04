using EstatePortal.Models;
using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public class UserRegistration
    {
        [Required(ErrorMessage = "Adres email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email.")]
        [Display(Name = "Adres email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
        [Display(Name = "Nazwa użytkownika")]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Hasło musi mieć co najmniej {2} znaków.", MinimumLength = 6)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hasła nie są identyczne.")]
        [Display(Name = "Potwierdź hasło")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Wybór roli jest wymagany.")]
        [Display(Name = "Rola użytkownika")]
        public UserRole Role { get; set; }

        // Dodatkowe pola w zależności od roli

        // Dla osoby prywatnej
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        // Dla biur nieruchomości i deweloperów
        [Display(Name = "Nazwa firmy")]
        public string CompanyName { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Phone(ErrorMessage = "Nieprawidłowy format numeru telefonu.")]
        [Display(Name = "Numer telefonu")]
        public string PhoneNumber { get; set; }

        // Jeśli rejestruje się pracownik, potrzebne jest ID pracodawcy
        [Display(Name = "ID pracodawcy")]
        public int? EmployerId { get; set; }
    }
}

