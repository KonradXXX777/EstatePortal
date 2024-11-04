using Microsoft.AspNetCore.Mvc;
using EstatePortal.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EstatePortal;

namespace EstatePortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Widok rejestracji osoby fizycznej
        [HttpGet]
        public IActionResult Register()
        {
			return View("~/Views/Home/Register.cshtml");
		}

        [HttpPost]
        public async Task<IActionResult> Register(UserRegister model)
        {
            if (ModelState.IsValid)
            {
                var salt = GenerateSalt();
                var hashedPassword = HashPassword(model.PasswordHash, salt);

                var newUser = new User
                {
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt,
                    Role = UserRole.PrivatePerson,
                    AcceptTerms = model.AcceptTerms,
                    DateRegistered = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
				return RedirectToAction("Index", "Home");
			}
            return View("~/Views/Home/Register.cshtml");
        }

        // Widok rejestracji Biura Nieruchomości
        [HttpGet]
        public IActionResult EstateAgencyRegister()
        {
			return View("~/Views/Home/EstateAgencyRegister.cshtml");
		}

        [HttpPost]
        public async Task<IActionResult> EstateAgencyRegister(EstateAgencyRegister model)
        {
            if (ModelState.IsValid)
            {
                var salt = GenerateSalt();
                var hashedPassword = HashPassword(model.PasswordHash, salt);

                var newUser = new User
                {
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt,
                    Role = UserRole.EstateAgency,
                    CompanyName = model.CompanyName,
                    NIP = model.NIP,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    AcceptTerms = model.AcceptTerms,
                    DateRegistered = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
				return RedirectToAction("Index", "Home");
			}
			return View("~/Views/Home/Register.cshtml");
		}

        // Widok rejestracji Dewelopera
        [HttpGet]
        public IActionResult DeveloperRegister()
        {
			return View("~/Views/Home/DeveloperRegister.cshtml");
		}

        [HttpPost]
        public async Task<IActionResult> DeveloperRegister(DeveloperRegister model)
        {
            if (ModelState.IsValid)
            {
                var salt = GenerateSalt();
                var hashedPassword = HashPassword(model.PasswordHash, salt);

                var newUser = new User
                {
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt,
                    Role = UserRole.Developer,
                    CompanyName = model.CompanyName,
                    NIP = model.NIP,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    AcceptTerms = model.AcceptTerms,
                    DateRegistered = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
				return RedirectToAction("Index", "Home");
			}
            return View("~/Views/Home/Register.cshtml", model);
        }

        private byte[] GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var combinedBytes = new byte[passwordBytes.Length + salt.Length];

                Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, salt.Length, passwordBytes.Length);

                return sha256.ComputeHash(combinedBytes);
            }
        }
        public bool VerifyPassword(string enteredPassword, byte[] storedHash, byte[] storedSalt)
        {
            var hash = HashPassword(enteredPassword, storedSalt);
            return hash.SequenceEqual(storedHash);
        }
    }
}
