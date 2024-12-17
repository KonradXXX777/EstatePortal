using EstatePortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
    public class AnnouncementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AnnouncementController(ApplicationDbContext context, IConfiguration configuration)
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SellOrRent()
        {
            return View();
        }

        // GET: AddApartment
        [HttpGet]
        public IActionResult AddAnnouncement(string propertyType, string sellOrRent)
        {
            // Przekazywanie info o rodzaju nieruchomosci z ViewData

            if (string.IsNullOrEmpty(propertyType) || string.IsNullOrEmpty(sellOrRent))
            {
                TempData["ErrorMessage"] = "Nie wybrano typu nieruchomości.";
                return View("SellOrRent");
            }
            ViewData["PropertyType"] = propertyType;
            ViewData["SellOrRent"] = sellOrRent;

            return View();
        }

        // Adding Announcement
        [HttpPost]
        public async Task<IActionResult> AddAnnouncement(Announcement model, AnnouncementFeature features, List<IFormFile> Photos)
        {
            //if (!ModelState.IsValid)
            //{
            //    Console.WriteLine("Model state is not valid");
            //    ModelState.AddModelError("", "Nieprawidłowe dane formularza.");
            //    return View(model);
            //}

            // Ustawienia użytkownika i czasu
            var userId = User.FindFirst("UserId")?.Value; // Zakładam, że UserId jest przechowywane w sesji lub tokenie
            if (userId == null)
            {
                ModelState.AddModelError("", "Błąd autoryzacji. Spróbuj ponownie.");
                return View(model);
            }

            model.UserId = int.Parse(userId);
            model.DateCreated = DateTime.UtcNow;

            // Dodanie ogłoszenia
            _context.Announcements.Add(model);
            await _context.SaveChangesAsync();

            // Dodanie cech ogłoszenia
            features.AnnouncementId = model.Id;
            _context.AnnouncementFeatures.Add(features);

            // Dodanie zdjęć
            foreach (var photo in Photos)
            {
                if (photo.Length > 0)
                {
                    // Przetwarzanie i zapisywanie zdjęcia
                    var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    _context.AnnouncementPhotos.Add(new AnnouncementPhoto
                    {
                        AnnouncementId = model.Id,
                        Url = $"/uploads/{fileName}"
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ogłoszenie zostało dodane pomyślnie.";
            return RedirectToAction("Index", "Home");
        }

// Wyświetlenie listy ogłoszeń użytkownika
[HttpGet]
        public async Task<IActionResult> MyAnnouncements()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var announcements = await _context.Announcements
                .Where(a => a.UserId == int.Parse(userId))
                .Include(a => a.Photos)
                .ToListAsync();

            return View(announcements);
        }

        // Szczegóły ogłoszenia
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var announcement = await _context.Announcements
                .Include(a => a.Features)
                .Include(a => a.Photos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (announcement == null)
            {
                return NotFound("Nie znaleziono ogłoszenia.");
            }

            return View(announcement);
        }
    }
}
