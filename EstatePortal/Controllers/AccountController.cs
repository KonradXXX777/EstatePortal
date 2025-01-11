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
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;


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

        // Error handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                    Status = UserStatus.Active,
                    AcceptTerms = model.AcceptTerms,
                    DateRegistered = DateTime.Now,
                    VerificationToken = verificationToken,
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
                var verificationToken = Guid.NewGuid().ToString(); // Token generation

                var newUser = new User
                {
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt,
                    Role = UserRole.EstateAgency,
                    Status = UserStatus.Active,
                    CompanyName = model.CompanyName,
                    NIP = model.NIP,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    AcceptTerms = model.AcceptTerms,
                    DateRegistered = DateTime.Now,
                    VerificationToken = verificationToken,
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                SendVerificationEmail(newUser.Email, verificationToken);
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
                var verificationToken = Guid.NewGuid().ToString(); // Token generation

                var newUser = new User
                {
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt,
                    Role = UserRole.Developer,
                    Status = UserStatus.Active,
                    CompanyName = model.CompanyName,
                    NIP = model.NIP,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    AcceptTerms = model.AcceptTerms,
                    DateRegistered = DateTime.Now,
                    VerificationToken = verificationToken,
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                SendVerificationEmail(newUser.Email, verificationToken);
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/Home/Register.cshtml", model);
        }

		// Google Register
		[HttpGet]
		public IActionResult LoginWithGoogle()
		{
			var redirectUrl = Url.Action("GoogleResponse", "Account");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}

		[HttpGet]
		public async Task<IActionResult> GoogleResponse()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			if (result?.Principal == null)
			{
				TempData["Error"] = "Nie udało się zalogować za pomocą Google.";
				return RedirectToAction("Login");
			}

			// Pobieranie danych z Google
			var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
			var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

			if (string.IsNullOrEmpty(email))
			{
				TempData["Error"] = "Nie udało się pobrać adresu e-mail.";
				return RedirectToAction("Login");
			}

			// Dodaj logikę: np. znajdź użytkownika w bazie lub utwórz nowe konto
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
			if (user == null)
			{
				user = new User
				{
					Email = email,
					FirstName = name?.Split(' ')[0],
					LastName = name?.Split(' ').Skip(1).FirstOrDefault(),
					Role = UserRole.PrivatePerson,
                    Status = UserStatus.Active,
                    DateRegistered = DateTime.Now
				};

				_context.Users.Add(user);
				await _context.SaveChangesAsync();
			}

			// Zalogowanie użytkownika
			var claims = new List<Claim>
	        {
		        new Claim(ClaimTypes.Name, user.FirstName ?? ""),
		        new Claim(ClaimTypes.Email, user.Email),
		        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
		        new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

			var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

			return RedirectToAction("UserPanel");
		}

		// Add Employee
		[HttpPost]
        public async Task<IActionResult> InviteEmployee(string email)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var employer = await _context.Users.FindAsync(int.Parse(userId));
            if (employer == null || (employer.Role != UserRole.EstateAgency && employer.Role != UserRole.Developer))
            {
                ModelState.AddModelError("", "Brak uprawnień do zapraszania pracowników.");
                return View("UserPanel");
            }

            // Tworzenie nowego zaproszenia
            var invitation = new EmployeeInvitation
            {
                Email = email,
                EmployerId = employer.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7) // Token wygasa po 7 dniach
            };

            _context.EmployeeInvitations.Add(invitation);
            await _context.SaveChangesAsync();

            // Tworzenie linku rejestracyjnego
            var registrationLink = Url.Action("EmployeeRegister", "Account", new { token = invitation.Token }, Request.Scheme);

            // Wysłanie e-maila z zaproszeniem
            await SendInvitationEmail(email, registrationLink);

            ViewBag.Message = "Zaproszenie zostało wysłane.";
            return View("UserPanel");
        }

        // Employee register
        [HttpGet]
        public async Task<IActionResult> EmployeeRegister(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token zaproszenia jest nieprawidłowy.");
            }

            var invitation = await _context.EmployeeInvitations.FirstOrDefaultAsync(i => i.Token == token);
            if (invitation == null || (invitation.ExpiryDate.HasValue && invitation.ExpiryDate.Value < DateTime.UtcNow))
            {
                return BadRequest("Zaproszenie wygasło lub jest nieprawidłowe.");
            }

            var model = new EmployeeRegister
            {
                Email = invitation.Email, // Preuzupełnienie e-maila z zaproszenia
                InvitationToken = invitation.Token
            };

            return View("~/Views/Home/EmployeeRegister.cshtml", model);
        }

        // Save employee user in DB
        [HttpPost]
        public async Task<IActionResult> EmployeeRegister(EmployeeRegister model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/EmployeeRegister.cshtml", model);
            }

            var invitation = await _context.EmployeeInvitations.FirstOrDefaultAsync(i => i.Token == model.InvitationToken);
            if (invitation == null || (invitation.ExpiryDate.HasValue && invitation.ExpiryDate.Value < DateTime.UtcNow))
            {
                ModelState.AddModelError("", "Zaproszenie wygasło lub jest nieprawidłowe.");
                return View("~/Views/Home/EmployeeRegister.cshtml", model);
            }

            var salt = GenerateSalt();
            var newUser = new User
            {
                Email = model.Email,
                PasswordHash = HashPassword(model.PasswordHash, salt),
                PasswordSalt = salt,
                Role = UserRole.Employee,
                Status = UserStatus.Active,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                EmployerId = invitation.EmployerId, // Powiązanie z pracodawcą
                DateRegistered = DateTime.Now,
                VerifiedAt = DateTime.Now,
                AcceptTerms = model.AcceptTerms
            };

            _context.Users.Add(newUser);

            // Usunięcie zaproszenia po rejestracji
            _context.EmployeeInvitations.Remove(invitation);

            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        // Invitation Email Sending
        private async Task SendInvitationEmail(string email, string link)
        {
            // Pobranie ustawień SMTP z pliku appsettings.json
            var emailSettings = _configuration.GetSection("SmtpSettings");
            var host = emailSettings["Host"];
            var port = int.Parse(emailSettings["Port"]);
            var senderName = emailSettings["SenderName"];
            var senderEmail = emailSettings["SenderEmail"];
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];

            // Tworzenie wiadomości e-mail z zaproszeniem
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "Zaproszenie do rejestracji w EstatePortal",
                Body = $"Witaj! Aby zarejestrować się, kliknij poniższy link:\n{link}",
                IsBodyHtml = false
            };

            mailMessage.To.Add(email);

            using (var client = new SmtpClient(host, port))
            {
                client.Credentials = new NetworkCredential(username, password);
                client.EnableSsl = true;

                try
                {
                    await client.SendMailAsync(mailMessage);
                    Console.WriteLine($"Wysłano zaproszenie do: {email}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas wysyłania zaproszenia e-mail: {ex.Message}");
                }
            }
        }

        // Invitation Employee Token Generating
        private async Task<string> GenerateAndSaveTokenForEmployee(string email, int employerId)
        {
            // Generowanie unikalnego tokenu
            var token = Guid.NewGuid().ToString();

            // Zapis tokenu do bazy danych dla danego pracownika (zakładam, że User zawiera pola na token)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.VerificationToken = token;
                user.EmployerId = employerId;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Dodajemy nowego użytkownika, jeśli nie istnieje
                var newUser = new User
                {
                    Email = email,
                    VerificationToken = token,
                    EmployerId = employerId,
                    Role = UserRole.Employee // Zakładamy, że nowy użytkownik to pracownik
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
            }
            return token;
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

            user.VerifiedAt = DateTime.Now;
            user.VerificationToken = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return View("~/Views/Home/VerificationSuccess.cshtml");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token resetujący jest wymagany.");
            }

            ViewData["Token"] = token;
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
            if (user == null || !VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt) || user.VerifiedAt == null)
            {
                ModelState.AddModelError("", "Nieprawidłowy e-mail lub hasło.");
                return View("~/Views/Home/Login.cshtml", model);
            }
            // Sprawdzanie statusu użytkownika
            if (user.Status == UserStatus.Blocked)
            {
                ModelState.AddModelError("", "Twoje konto zostało zablokowane. Skontaktuj się z administratorem.");
                return View("~/Views/Home/Login.cshtml", model);
            }

            if (user.Status == UserStatus.Inactive)
            {
                ModelState.AddModelError("", "Twoje konto jest nieaktywne.");
                return View("~/Views/Home/Login.cshtml", model);
            }

            // Tworzenie tożsamości użytkownika
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName ?? ""),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("UserId", user.Id.ToString()), // Maybe valuable
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync("Cookies", claimsPrincipal);
            //return View("UserPanel");
            return RedirectToAction("Index", "Home");
        }

        // Logout user
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> UserPanel()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Ustawienie ViewBag dla roli i statusu użytkownika
            ViewBag.UserRole = user.Role;
            ViewBag.UserStatus = user.Status;

            var notifications = _context.Notifications
            .Where(n => n.UserId == user.Id && !n.IsRead)   // np. nieprzeczytane
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

            ViewBag.Notifications = notifications;

            var model = new UserUpdate
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CompanyName = user.CompanyName,
                NIP = user.NIP,
                Address = user.Address
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UserPanel(UserUpdate model)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Aktualizacja danych użytkownika
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.CompanyName = model.CompanyName;
            user.NIP = model.NIP;
            user.Address = model.Address;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Ustawienie ViewBag dla roli i statusu użytkownika
            ViewBag.UserRole = user.Role;
            ViewBag.UserStatus = user.Status;

            ViewBag.Message = "Dane zostały zaktualizowane.";
            return RedirectToAction("UserPanel");
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserUpdate model)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Weryfikacja obecnego hasła
            if (!VerifyPassword(model.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            {
                ModelState.AddModelError("", "Obecne hasło jest nieprawidłowe.");
                return View("UserPanel", model);
            }

            // Aktualizacja hasła
            var salt = GenerateSalt();
            user.PasswordHash = HashPassword(model.NewPassword, salt);
            user.PasswordSalt = salt;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Hasło zostało zmienione.";
            return View("UserPanel", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Zmiana statusu na Inactive
            user.Status = UserStatus.Inactive;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Opcjonalnie: Wylogowanie użytkownika po dezaktywacji
            await HttpContext.SignOutAsync();

            // Po usunięciu konta, przekierowanie na stronę logowania
            return RedirectToAction("Login");
        }

        // Admin Panel (GET)
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult AdminPanel(int? editUserId = null, int? editAnnouncementId = null, AnnouncementStatus? filterStatus = null)
        {
            var currentUserRole = User.IsInRole("Administrator") ? "Administrator" : "Moderator";

            var users = _context.Users.AsQueryable();
            var announcements = _context.Announcements
                .Include(a => a.Photos) // Ładuj zdjęcia razem z ogłoszeniami
                .AsQueryable();

            if (currentUserRole == "Moderator")
            {
                users = users.Where(u => u.Role != UserRole.Administrator && u.Role != UserRole.Moderator);
            }
            if (filterStatus.HasValue)
            {
                announcements = announcements.Where(a => a.Status == filterStatus.Value);
            }

            User userToEdit = null;
            if (editUserId.HasValue)
            {
                userToEdit = _context.Users.FirstOrDefault(u => u.Id == editUserId.Value);
            }

            Announcement announcementToEdit = null;
            if (editAnnouncementId.HasValue)
            {
                announcementToEdit = _context.Announcements.FirstOrDefault(a => a.Id == editAnnouncementId.Value);
            }

            var viewModel = new AdminPanelViewModel
            {
                Users = users.ToList(),
                Announcements = announcements.ToList(),
                EditUser = userToEdit,
                EditAnnouncement = announcementToEdit
            };

            return View(viewModel);
        }

        // Update User (POST)
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UpdateUser(User model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
                return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.CompanyName = model.CompanyName;
            user.NIP = model.NIP;
            user.Address = model.Address;
            user.Status = model.Status;

            _context.SaveChanges();
            return RedirectToAction("AdminPanel");
        }

        // Change User Status (POST)
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult ChangeUserStatus(int id, UserStatus status)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            user.Status = status;
            _context.SaveChanges();
            return RedirectToAction("AdminPanel");
        }

        // Update Announcement (POST)
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UpdateAnnouncement(Announcement model)
        {
            var announcement = _context.Announcements.FirstOrDefault(a => a.Id == model.Id);
            if (announcement == null)
                return NotFound();

            // Aktualizacja danych ogłoszenia
            announcement.Title = model.Title;
            announcement.Description = model.Description;
            announcement.PropertyType = model.PropertyType;
            announcement.SellOrRent = model.SellOrRent;

            _context.SaveChanges();
            return RedirectToAction("AdminPanel");
        }

        // Change Announcement Status (POST)
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult ChangeAnnouncementStatus(int id, AnnouncementStatus status)
        {
            var announcement = _context.Announcements.FirstOrDefault(a => a.Id == id);
            if (announcement == null)
                return NotFound();

            announcement.Status = status;
            _context.SaveChanges();
            return RedirectToAction("AdminPanel");
        }
    }
}