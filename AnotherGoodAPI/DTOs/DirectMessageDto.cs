namespace AnotherGoodAPI.DTOs
{
    public class DirectMessageDto
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public int? ParentMessageId { get; set; }
    }
}
