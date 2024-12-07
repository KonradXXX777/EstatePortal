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

            if (chat == null) return NotFound();

            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int chatId, string message)
        {
            var senderId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var chat = await _context.Chats.FindAsync(chatId);

            if (chat == null) return NotFound();

            var newMessage = new Message
            {
                ChatId = chatId,
                SenderId = senderId,
                Content = message,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(newMessage);
            chat.LastMessage = message;
            chat.LastMessageTime = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("ChatRoom", new { chatId });
        }

        [HttpGet]
        public async Task<IActionResult> StartChat(int announcementId)
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

            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> StartChat(Chat chat)
        {
            if (!ModelState.IsValid)
            {
                return View(chat); // Walidacja nie powiodła się, wróć do widoku
            }

            var senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Identyfikator zalogowanego użytkownika
            chat.SenderId = senderId;
            chat.LastMessageTime = DateTime.Now;

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
