using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.DTOs
{
    public class DirectMessageCreateDto
    {
        [Required]
        public string Body { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        public int? ParentMessageId { get; set; }
    }
}
