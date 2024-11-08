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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


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

        // Private persons
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Home/Register.cshtml");
        }

        [AllowAnonymous]
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
        [AllowAnonymous]
        [HttpGet]
        public IActionResult EstateAgencyRegister()
        {
            return View("~/Views/Home/EstateAgencyRegister.cshtml");
        }

        [AllowAnonymous]
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
        [AllowAnonymous]
        [HttpGet]
        public IActionResult DeveloperRegister()
        {
            return View("~/Views/Home/DeveloperRegister.cshtml");
        }

        [AllowAnonymous]
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
        [AllowAnonymous]
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

            user.VerifiedAt = DateTime.Now; // Set date
            user.VerificationToken = null; // Token removal after verification
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return View("~/Views/Home/VerificationSuccess.cshtml");
        }

        // GET displays the form, POST saves new password
        /*
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token resetujący jest wymagany.");
            }

            var model = new ResetPassword { Token = token };
            return View("~/Views/Home/ResetPassword.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult>ResetPassword(ResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/ResetPassword.cshtml", model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == model.Token && u.ResetTokenExpiry > DateTime.Now);
            if (user == null)
            {
                ModelState.AddModelError("", "Token resetujący jest nieważny lub wygasł.");
                return View("~/Views/Home/ResetPassword.cshtml", model);
            }

            var salt = GenerateSalt();
            user.PasswordHash = HashPassword(model.NewPassword, salt);
            user.PasswordSalt = salt;
            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;
            user.PasswordLastReset = DateTime.Now;

            await _context.SaveChangesAsync();

            //return RedirectToAction("~/Views/Home/Register.cshtml");

            //return View("~/Views/Home/Login.cshtml");
            return RedirectToAction("Login", "Home");
        } */

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token resetujący jest wymagany.");
            }

            ViewData["Token"] = token; // Przechowujemy token w ViewData
            return View("~/Views/Home/ResetPassword.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Hasła się nie zgadzają.");
                ViewData["Token"] = token;
                return View("~/Views/Home/ResetPassword.cshtml");
                // return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token && u.ResetTokenExpiry > DateTime.Now);
            if (user == null)
            {
                ModelState.AddModelError("", "Token resetujący jest nieważny lub wygasł.");
                ViewData["Token"] = token;
                return View("~/Views/Home/ResetPassword.cshtml");
                //return View();
            }

            var salt = GenerateSalt();
            user.PasswordHash = HashPassword(newPassword, salt);
            user.PasswordSalt = salt;
            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;
            user.PasswordLastReset = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Home");
            // return View();
        }


        // Generating password reset link and sending email
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "Nie znaleziono użytkownika o podanym adresie e-mail.");
                return View("~/Views/Home/ForgotPassword.cshtml");
            }

            // Generowanie tokenu resetującego
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiry = DateTime.Now.AddHours(1); // Token ważny przez 1 godzinę
            await _context.SaveChangesAsync();

            // Wysyłanie e-maila z linkiem resetującym hasło
            SendPasswordResetEmail(user.Email, user.PasswordResetToken);

            return View("~/Views/Home/Register.cshtml"); // Widok potwierdzający wysłanie e-maila
        }

        private void SendPasswordResetEmail(string userEmail, string resetToken)
        {
            var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken }, Request.Scheme);
            var message = $"Aby zresetować swoje hasło, kliknij w poniższy link:\n{resetLink}";

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
                Subject = "Resetowanie hasła - Estate Portal",
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
                    Console.WriteLine($"Wysłano e-mail resetujący hasło do: {userEmail}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd wysyłania e-maila: {ex.Message}");
                }
            }
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

        // User Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Home/Login.cshtml");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Login.cshtml", model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                ModelState.AddModelError("", "Nieprawidłowy e-mail lub hasło.");
                return View("~/Views/Home/Login.cshtml", model);
            }

            // Logika logowania użytkownika: zapisujemy ID użytkownika w sesji
            HttpContext.Session.SetString("UserId", user.Id.ToString()); // Dodać sesje w Program.cs

            return RedirectToAction("UserPanel");
        }

        // Logout user
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }

        // User Dashboard
        [Authorize]
        [HttpGet]
        public IActionResult UserPanel()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login");
            }
            return View("UserPanel");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserDashboard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Login");

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return RedirectToAction("Login");

            return View("UserPanel", user);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateUserData(User model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/UserPanel.cshtml", model);
            }

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Login");

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return RedirectToAction("Login");

            // Aktualizacja danych
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Dane zaktualizowane pomyślnie.";
            return View("~/Views/Home/UserPanel.cshtml", user);
        }
    }
}
