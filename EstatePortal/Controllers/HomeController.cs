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
            // Pobierz email z ClaimsIdentity
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            Console.WriteLine("Email z claims: " + email);

            if (!string.IsNullOrEmpty(email))
            {
                // Pobierz u¿ytkownika z bazy danych na podstawie adresu e-mail
                var user = _context.Users.FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    // Przeka¿ status u¿ytkownika do widoku
                    ViewBag.UserStatus = user.Status;

                    // Przeka¿ komunikat zale¿nie od statusu
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
                }
                else
                {
                    // Jeœli u¿ytkownik nie istnieje w bazie, wyœwietl komunikat
                    ViewBag.StatusMessage = "Nie znaleziono u¿ytkownika.";
                }
            }
            else
            {
                // Jeœli u¿ytkownik nie jest zalogowany
                ViewBag.StatusMessage = "Nie jesteœ zalogowany.";
            }

            // Zawsze zwróæ widok
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
