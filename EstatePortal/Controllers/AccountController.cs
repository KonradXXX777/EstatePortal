using Microsoft.AspNetCore.Mvc;
using EstatePortal.Models;
using EstatePortal;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EstatePortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            //return View();
            return View("~/Views/Home/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string role, UserRegister model, UserRegister userRegister, EstateAgencyRegister estateAgencyRegister, DeveloperRegister developerRegister)
        {
            if (ModelState.IsValid)
            {
                // Sprawdzenie, czy użytkownik o takim samym adresie email już istnieje
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Użytkownik o podanym adresie email już istnieje.");
                    //return View(model);
                    return View("~/Views/Home/Register.cshtml");
                }

                // Zmienna do przechowywania nowo utworzonego użytkownika
                User newUser = null;

                // Wybór odpowiedniego modelu w zależności od wybranej roli
                switch (role)
                {
                    case "PrivatePerson":
                        newUser = new User
                        {
                            Email = userRegister.Email,
                            PasswordHash = HashPassword(userRegister.PasswordHash), // Używamy funkcji do hashowania
                            Role = UserRole.PrivatePerson,
                            AcceptTerms = userRegister.AcceptTerms
                        };
                        break;
                   
                    case "EstateAgency":
                            newUser = new User
                            {
                                //Email = estateAgencyModel.Email,
                                //PasswordHash = HashPassword(estateAgencyModel.PasswordHash),
                                //Role = UserRole.EstateAgency,
                                //AcceptTerms = estateAgencyModel.AcceptTerms,
                                //CompanyName = estateAgencyModel.CompanyName,
                                //NIP = estateAgencyModel.NIP,
                                //Address = estateAgencyModel.Address,
                                //PhoneNumber = estateAgencyModel.PhoneNumber
                                Email = estateAgencyRegister.Email,
                                PasswordHash = HashPassword(estateAgencyRegister.PasswordHash),
                                Role = UserRole.EstateAgency,
                                AcceptTerms = estateAgencyRegister.AcceptTerms,
                                CompanyName = estateAgencyRegister.CompanyName,
                                NIP = estateAgencyRegister.NIP,
                                Address = estateAgencyRegister.Address,
                                PhoneNumber = estateAgencyRegister.PhoneNumber
                            };
                        break;

                    case "Developer":
                            newUser = new User
                            {
                                Email = developerRegister.Email,
                                PasswordHash = HashPassword(developerRegister.PasswordHash),
                                Role = UserRole.Developer,
                                AcceptTerms = developerRegister.AcceptTerms,
                                CompanyName = developerRegister.CompanyName,
                                NIP = developerRegister.NIP,
                                Address = developerRegister.Address,
                                PhoneNumber = developerRegister.PhoneNumber
                            };
                        break;
                   
                    default:
                        ModelState.AddModelError("", "Nieprawidłowa rola użytkownika.");
                        //return View(model);
                        return View("~/Views/Home/Register.cshtml");
                }

                // Zapis użytkownika do bazy danych
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync(); // Asynchroniczny zapis

                // Po rejestracji przekierowanie na stronę główną lub logowania
                return RedirectToAction("Index", "Home");
            }

            // Jeśli model nie jest prawidłowy, wróć do widoku formularza z walidacją
            //return View(model);
            return View("~/Views/Home/Register.cshtml");
        }

        // Funkcja do hashowania hasła z użyciem algorytmu SHA256
        private byte[] HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password)); // Zwraca tablicę bajtów (byte[])
            }
        }
    }
}

