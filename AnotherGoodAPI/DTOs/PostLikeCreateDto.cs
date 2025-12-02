using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.DTOs
{
    public class PostLikeCreateDto
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
