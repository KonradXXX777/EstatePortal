using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstatePortal.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace EstatePortal.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> ChatHistory()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var chats = await _context.Chats
                .Include(c => c.Receiver)
                .Include(c => c.Announcement)
                .Where(c => c.SenderId == userId || c.ReceiverId == userId)
                .ToListAsync();

            return View(chats);
        }

        public async Task<IActionResult> ChatRoom(int chatId)
        {
            var chat = await _context.Chats
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Include(c => c.Announcement)
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null) return NotFound("Nie znaleziono czatu.");

            var messages = await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            ViewBag.Messages = messages;

            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int chatId, string message)
        {
            var senderId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var chat = await _context.Chats.FindAsync(chatId);

            if (chat == null) return NotFound("Nie znaleziono czatu.");

            var newMessage = new Message
            {
                ChatId = chatId,
                SenderId = senderId,
                ReceiverId = chat.ReceiverId,
                Content = message,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            chat.LastMessage = message;
            chat.LastMessageTime = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction("ChatRoom", new { chatId });
        }

        [HttpGet]
        public async Task<IActionResult> NewChat(int announcementId)
        {
            var announcement = await _context.Announcements
                .Include(a => a.User) // Pobranie właściciela ogłoszenia
                .FirstOrDefaultAsync(a => a.Id == announcementId);

            if (announcement == null)
            {
                return NotFound("Ogłoszenie nie zostało znalezione.");
            }

            var chat = new Chat
            {
                AnnouncementId = announcementId,
                ReceiverId = announcement.UserId // Właściciel ogłoszenia
            };

            if (!User.Identity.IsAuthenticated)
            {
                Console.WriteLine("Uzytkownik niezalogowany.");
            }
            else
            {
                Console.WriteLine("Uzytkownik zalogowany");
            }

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(claimValue))
            {
                Console.WriteLine("Brak identyfikatora użytkownika1");
                return BadRequest("Brak identyfikatora użytkownika.");
            }

            var senderId = int.Parse(claimValue);

            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> NewChat(Chat chat)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(chat); // Walidacja nie powiodła się, wróć do widoku
            //}

            var senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Identyfikator zalogowanego użytkownika

            chat.SenderId = senderId;
            chat.LastMessageTime = DateTime.Now;

            if (chat.SenderId == 0 || chat.ReceiverId == 0 || chat.AnnouncementId == 0)
            {
                return BadRequest("Brakuje wymaganych identyfikatorów.");
            }

            // Przykład sprawdzenia użytkowników w bazie
            var senderExists = await _context.Users.AnyAsync(u => u.Id == chat.SenderId);
            var receiverExists = await _context.Users.AnyAsync(u => u.Id == chat.ReceiverId);

            if (!senderExists || !receiverExists)
            {
                return BadRequest("Jeden z użytkowników nie istnieje.");
            }

            // Sprawdź, czy czat już istnieje
            var existingChat = await _context.Chats.FirstOrDefaultAsync(c =>
                c.SenderId == senderId &&
                c.ReceiverId == chat.ReceiverId &&
                c.AnnouncementId == chat.AnnouncementId);

            if (existingChat == null)
            {
                _context.Chats.Add(chat);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ChatHistory");
        }



    }
}
