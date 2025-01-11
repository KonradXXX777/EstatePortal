namespace EstatePortal.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }          // do kogo to powiadomienie
        public string Message { get; set; }      // np. "Nowa wiadomość od ..."
        public bool IsRead { get; set; } = false;
        public int ChatId { get; set; }          // w którym czacie
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
