using EstatePortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace EstatePortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            Console.WriteLine("Email z claims: " + email);

            if (!string.IsNullOrEmpty(email))
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    ViewBag.UserStatus = user.Status;
                    ViewBag.UserRole = user.Role;

                    switch (user.Status)
                    {
                        case UserStatus.Blocked:
                            ViewBag.StatusMessage = "Twoje konto jest zablokowane.";
                            break;
                        case UserStatus.Inactive:
                            ViewBag.StatusMessage = "Twoje konto jest nieaktywne.";
                            break;
                        case UserStatus.Active:
                            ViewBag.StatusMessage = "Twoje konto jest aktywne.";
                            break;
                        default:
                            ViewBag.StatusMessage = "Nieznany status konta.";
                            break;
                    }

                    ViewBag.LoginStatusMessage = "Jesteœ zalogowany.";

                    // Pobieramy powiadomienia
                    var notifications = _context.Notifications
                        .Where(n => n.UserId == user.Id && !n.IsRead)
                        .OrderByDescending(n => n.CreatedAt)
                        .ToList();

                    // Dodajemy e-mail nadawcy do powiadomienia
                    foreach (var notif in notifications)
                    {
                        // Znajdujemy u¿ytkownika, który wys³a³ wiadomoœæ
                        var message = _context.Messages.FirstOrDefault(m => m.ChatId == notif.ChatId && m.SenderId != user.Id);
                        if (message != null)
                        {
                            // Przypisujemy e-mail nadawcy do powiadomienia
                            var senderEmail = _context.Users.FirstOrDefault(u => u.Id == message.SenderId)?.Email;
                            if (senderEmail != null)
                            {
                                notif.Message = $"Dosta³eœ wiadomoœæ od {senderEmail}";
                            }
                        }
                    }

                    ViewBag.Notifications = notifications;
                }
                else
                {
                    ViewBag.StatusMessage = "Nie znaleziono u¿ytkownika.";
                    ViewBag.LoginStatusMessage = "Jesteœ zalogowany, ale konto nie istnieje w bazie.";
                }
            }
            else
            {
                ViewBag.StatusMessage = "Nie jesteœ zalogowany.";
                ViewBag.LoginStatusMessage = "Nie jesteœ zalogowany.";
            }

            return View();
        }






        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult EstateAgencyRegister()
        {
            return View();
        }
        public IActionResult DeveloperRegister()
        {
            return View();
        }
        public IActionResult VerificationSuccess()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult ResetPassword()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
