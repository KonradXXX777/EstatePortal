using Microsoft.AspNetCore.Mvc;
using EstatePortal.Models;
using EstatePortal;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
/*
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegister model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FindAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Użytkownik o podanym adresie email już istnieje.");
                    return View(model);
                    
                }

                var newUser = new User
                {
                    Email = model.Email,
                    Name = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    
                };

                newUser.PasswordHash = HashPassword(model.Password);
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
*/