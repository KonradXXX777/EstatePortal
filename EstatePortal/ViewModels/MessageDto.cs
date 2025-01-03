namespace EstatePortal.ViewModels
{
    public class MessageDto
    {
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
