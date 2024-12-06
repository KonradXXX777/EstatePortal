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

        // GET: AddApartment
        [HttpGet]
        public IActionResult AddApartment()
        {
            return View();
        }

        // POST: AddApartment
        [HttpPost]
        public IActionResult AddApartment(Announcement announcement, List<IFormFile> MultimediaFiles)
        {
            if (ModelState.IsValid)
            {
                // Tutaj możesz dodać logikę do zapisania pliku do bazy danych lub systemu plików

                // Przekieruj do innej strony po zapisaniu
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
