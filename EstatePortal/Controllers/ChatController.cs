using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstatePortal.Models;
using EstatePortal.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using EstatePortal.Hubs;
using Microsoft.AspNetCore.SignalR;

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

        [HttpGet]
        public IActionResult GoToChatRoom(int announcementId, int receiverId)
        {
            var senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Checks whether the chat exists
            var chat = _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefault(c =>
                    c.AnnouncementId == announcementId &&
                    ((c.SenderId == senderId && c.ReceiverId == receiverId) ||
                     (c.SenderId == receiverId && c.ReceiverId == senderId))
                );

            if (chat == null)
            {
                chat = new Chat
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    AnnouncementId = announcementId,
                    LastMessageTime = DateTime.Now
                };
                _context.Chats.Add(chat);
                _context.SaveChanges();
            }

            return RedirectToAction("ChatRoom", new { chatId = chat.Id });
        }

        [HttpGet]
        public IActionResult ChatRoom(int chatId)
        {
            // Pobieramy czat z bazy
            var chat = _context.Chats
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .FirstOrDefault(c => c.Id == chatId);

            if (chat == null)
            {
                return NotFound();
            }

            return View(chat);
        }

        [HttpPost]
        public IActionResult SaveMessage([FromBody] MessageDto data)
        {
            var message = new Message
            {
                ChatId = data.ChatId,
                SenderId = data.SenderId,
                ReceiverId = data.ReceiverId,
                Content = data.Content,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(message);

            var chat = _context.Chats.Find(data.ChatId);
            if (chat != null)
            {
                chat.LastMessageTime = DateTime.Now;
            }

            _context.SaveChanges();
            return Ok();
        }
    }
}
