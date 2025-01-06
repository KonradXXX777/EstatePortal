using EstatePortal.Models;
using EstatePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using EstatePortal.Services;
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
using EstatePortal.Services;

namespace EstatePortal.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ImageDetectionService _imageDetectionService;

        public AnnouncementController(ApplicationDbContext context, IConfiguration configuration, ImageDetectionService detectionService)
        {
            _context = context;
            _configuration = configuration;
            _imageDetectionService = detectionService;
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
        public async Task<IActionResult> AddAnnouncement(
            Announcement model,
            AnnouncementFeature features,
            List<IFormFile> Photos)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                ModelState.AddModelError("", "Błąd autoryzacji. Spróbuj ponownie.");
                return View(model);
            }

            model.UserId = int.Parse(userId);
            model.DateCreated = DateTime.UtcNow;

            // Domyślnie dodajemy ogłoszenie do kontekstu i zapisujemy
            _context.Announcements.Add(model);
            await _context.SaveChangesAsync();

            // Dodanie cech
            features.AnnouncementId = model.Id;
            _context.AnnouncementFeatures.Add(features);

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var currentUser = await _context.Users.FindAsync(model.UserId);
            if (currentUser == null)
            {
                ModelState.AddModelError("", "Nie znaleziono użytkownika w bazie.");
                return View(model);
            }

            AnnouncementStatus finalStatus = AnnouncementStatus.Active;

            foreach (var photo in Photos)
            {
                // Sprawdzamy rozszerzenie / rozmiar
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".heic", ".heif" };
                var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    Console.WriteLine($"File {photo.FileName} rejected: Invalid format");
                    continue;
                }
                if (photo.Length > 10 * 1024 * 1024)
                {
                    Console.WriteLine($"File {photo.FileName} rejected: File size exceeds 10 MB");
                    continue;
                }

                // 1. Analiza zdjęcia modelem
                using (var memoryStream = new MemoryStream())
                {
                    await photo.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    float detectionScore = _imageDetectionService.CheckForbiddenObjects(memoryStream);

                    // Zależnie od progu zmieniamy finalStatus
                    if (detectionScore >= 0.8f)
                    {
                        // Ogłoszenie odrzucone
                        finalStatus = AnnouncementStatus.Rejected;
                    }
                    else if (detectionScore >= 0.75f)
                    {
                        // Tylko jeśli nie jest Rejected (Rejected ma "wyższy priorytet")
                        if (finalStatus != AnnouncementStatus.Rejected)
                        {
                            finalStatus = AnnouncementStatus.PendingApproval;
                        }
                    }
                }

                // 2. Niezależnie od statusu - zapisujemy zdjęcie
                var fileName = Guid.NewGuid() + extension;
                var filePath = Path.Combine(uploadPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    using var readStream = photo.OpenReadStream();
                    readStream.Position = 0;
                    await readStream.CopyToAsync(fileStream);
                }

                _context.AnnouncementPhotos.Add(new AnnouncementPhoto
                {
                    AnnouncementId = model.Id,
                    Url = $"/uploads/{fileName}"
                });
            }

            // Po przejściu wszystkich zdjęć - ustawiamy finalny status ogłoszenia
            model.Status = finalStatus;

            // Zapisujemy ostateczny status + zdjęcia w bazie
            await _context.SaveChangesAsync();

            if (finalStatus == AnnouncementStatus.Rejected)
            {
                TempData["ErrorMessage"] = "Wykryto niedozwolone treści. Ogłoszenie zostało odrzucone.";
                // Możesz przekierować do "MyAnnouncements" lub innego widoku
                return RedirectToAction("MyAnnouncements", "Announcement");
            }
            else if (finalStatus == AnnouncementStatus.PendingApproval)
            {
                TempData["WarningMessage"] = "Ogłoszenie oczekuje na weryfikację (Pending Approval).";
            }
            else
            {
                TempData["SuccessMessage"] = "Ogłoszenie zostało dodane pomyślnie (Active).";
            }

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
        public async Task<IActionResult> AnnouncementDetails(int id)
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
        // Widok edycji ogłoszenia
        public IActionResult AnnouncementDetailsEdit(int id)
        {
            var announcement = _context.Announcements
                .Include(a => a.Features)
                .FirstOrDefault(a => a.Id == id);

            if (announcement == null)
            {
                return NotFound();
            }

            return View(announcement);
        }

        // Akcja zapisywania zmian
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AnnouncementDetailsEdit(int id, Announcement model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            var announcement = _context.Announcements
                .Include(a => a.Features)
                .FirstOrDefault(a => a.Id == id);

            if (announcement == null)
            {
                return NotFound();
            }

            // Przypisz wartości tylko wtedy, gdy są niepuste lub niezerowe
            if (!string.IsNullOrEmpty(model.Title))
                announcement.Title = model.Title;
            if (!string.IsNullOrEmpty(model.Location))
                announcement.Location = model.Location;
            if (!string.IsNullOrEmpty(model.Street))
                announcement.Street = model.Street;
            if (model.Area > 0)
                announcement.Area = model.Area;
            if (model.Price > 0)
                announcement.Price = model.Price;
            if (!string.IsNullOrEmpty(model.Description))
                announcement.Description = model.Description;
            if (Enum.TryParse<SellOrRent>(model.SellOrRent.ToString(), out var sellOrRent))
                announcement.SellOrRent = sellOrRent;
            if (Enum.TryParse<PropertyType>(model.PropertyType.ToString(), out var propertyType))
                announcement.PropertyType = propertyType;
            if (Enum.TryParse<AnnouncementStatus>(model.Status.ToString(), out var status))
                announcement.Status = status;

            // Edytuj cechy nieruchomości (Features)
            if (announcement.Features != null && model.Features != null)
            {
                if (model.Features.Rooms > 0)
                    announcement.Features.Rooms = model.Features.Rooms;
                if (model.Features.Floor > 0)
                    announcement.Features.Floor = model.Features.Floor;
                if (model.Features.TotalFloors > 0)
                    announcement.Features.TotalFloors = model.Features.TotalFloors;
                if (model.Features.YearBuilt > 0)
                    announcement.Features.YearBuilt = model.Features.YearBuilt;
                announcement.Features.Elevator = model.Features.Elevator;
                announcement.Features.IsAccessible = model.Features.IsAccessible;
                announcement.Features.HasGarage = model.Features.HasGarage;
                announcement.Features.HasGarden = model.Features.HasGarden;
                announcement.Features.HasBasement = model.Features.HasBasement;
                announcement.Features.HasAirConditioning = model.Features.HasAirConditioning;
                announcement.Features.HasWater = model.Features.HasWater;
                announcement.Features.HasElectricity = model.Features.HasElectricity;
                announcement.Features.HasGas = model.Features.HasGas;
                announcement.Features.HasSewerage = model.Features.HasSewerage;
                announcement.Features.HasInternet = model.Features.HasInternet;
                announcement.Features.HasForest = model.Features.HasForest;
                announcement.Features.HasPark = model.Features.HasPark;
                announcement.Features.HasSea = model.Features.HasSea;
                announcement.Features.HasMountains = model.Features.HasMountains;
            }

            try
            {
                _context.Update(announcement);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Announcements.Any(a => a.Id == announcement.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(AnnouncementDetails), new { id = announcement.Id });
            //}
        }

        [HttpGet]
        public async Task<IActionResult> Listing(ListingFilterViewModel model)
        {
            var query = _context.Announcements
                .Include(a => a.User)
                .Include(a => a.Photos)
                .Where(a => a.Status == AnnouncementStatus.Active)
                .AsQueryable();

            // Filtrowanie na podstawie wyszukiwanej frazy (tytuł lub opis)
            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(a => a.Title.Contains(model.Search)
                                      || a.Description.Contains(model.Search));
            }

            // Filtrowanie na podstawie lokalizacji
            if (!string.IsNullOrEmpty(model.Location))
            {
                query = query.Where(a => a.Location.Contains(model.Location));
            }

            // Filtrowanie ceny
            if (model.MinPrice.HasValue)
            {
                query = query.Where(a => a.Price >= model.MinPrice.Value);
            }
            if (model.MaxPrice.HasValue)
            {
                query = query.Where(a => a.Price <= model.MaxPrice.Value);
            }

            // Filtrowanie powierzchni
            if (model.MinArea.HasValue)
            {
                query = query.Where(a => a.Area >= model.MinArea.Value);
            }
            if (model.MaxArea.HasValue)
            {
                query = query.Where(a => a.Area <= model.MaxArea.Value);
            }

            // Filtrowanie rodzaju nieruchomości
            if (model.PropertyType.HasValue)
            {
                query = query.Where(a => a.PropertyType == model.PropertyType.Value);
            }

            // Filtrowanie typu użytkownika
            if (model.UserRole.HasValue)
            {
                query = query.Where(a => a.User.Role == model.UserRole.Value);
            }

            // Filtrowanie rodzaju oferty (Sprzedaż/Wynajem)
            if (model.SellOrRent.HasValue)
            {
                query = query.Where(a => a.SellOrRent == model.SellOrRent.Value);
            }

            // Ściągamy wyniki do listy
            model.Announcements = await query.ToListAsync();

            // Zwracamy widok wraz z modelem
            return View(model);
        }

        public async Task<IActionResult> ListingDetails(int id)
        {
            var announcement = await _context.Announcements
                .Include(a => a.Features) // Wczytanie powiązanych cech
                .Include(a => a.Photos)   // Wczytanie powiązanych zdjęć
                .FirstOrDefaultAsync(a => a.Id == id);

            if (announcement == null)
            {
                return NotFound(); // Jeśli ogłoszenie nie zostanie znalezione, zwróć 404
            }

            return View(announcement); // Przekazanie pojedynczego ogłoszenia do widoku
        }

    }
}
