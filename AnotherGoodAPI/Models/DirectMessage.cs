using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.Models
{
    public class DirectMessage
    {
        public int Id { get; set; }

        [Required]
        public string Body { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        // Relations
        [Required]
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }

        [Required]
        public string ReceiverId { get; set; }
        public ApplicationUser Receiver { get; set; }

        // Optional: parent message for replies
        public int? ParentMessageId { get; set; }
        public DirectMessage ParentMessage { get; set; }
    }
}
