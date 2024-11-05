using Microsoft.AspNetCore.Mvc;
using EstatePortal.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EstatePortal;
using System.Configuration;
using System.Net.Mail;
using System.Net;


namespace EstatePortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        //Private persons
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
                var verificationToken = Guid.NewGuid().ToString(); // Token generation

                var newUser = new User
                {
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt,
                    Role = UserRole.PrivatePerson,
                    AcceptTerms = model.AcceptTerms,
                    DateRegistered = DateTime.Now,
                    VerificationToken = verificationToken
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                SendVerificationEmail(newUser.Email, verificationToken);
                return RedirectToAction("Index", "Home");
			}
            return View("~/Views/Home/Register.cshtml");
        }

        //Estate Agencies
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

        //Developers
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

        // Read SMTP configuration
        private void SendVerificationEmail(string userEmail, string verificationToken)
        {
            var verificationLink = Url.Action("VerifyEmail", "Account", new { token = verificationToken }, Request.Scheme);
            var message = $"Witaj! Aby zweryfikować swoje konto, kliknij w link poniżej:\n{verificationLink}";

            var emailSettings = _configuration.GetSection("SmtpSettings");
            var host = emailSettings["Host"];
            var port = int.Parse(emailSettings["Port"]);
            var senderName = emailSettings["SenderName"];
            var senderEmail = emailSettings["SenderEmail"];
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "Weryfikacja konta - Estate Portal",
                Body = message,
                IsBodyHtml = false
            };

            mailMessage.To.Add(userEmail);

            using (var client = new SmtpClient(host, port))
            {
                client.Credentials = new NetworkCredential(username, password);
                client.EnableSsl = true;

                try
                {
                    client.Send(mailMessage);
                    Console.WriteLine($"Email weryfikacyjny wysłany do: {userEmail}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd wysyłania e-maila: {ex.Message}");
                }
            }
        }

        // User verify service (link in email) 
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token weryfikacyjny jest wymagany.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

            if (user == null)
            {
                return NotFound("Nie znaleziono użytkownika z podanym tokenem.");
            }

            user.VerifiedAt = DateTime.Now; // Ustawienie daty weryfikacji
            user.VerificationToken = null; // Usunięcie tokenu po weryfikacji
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return View("~/Views/Home/VerificationSuccess.cshtml"); // Widok potwierdzenia sukcesu
        }

        //Password Salting
        private byte[] GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        //Password Hashing
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
