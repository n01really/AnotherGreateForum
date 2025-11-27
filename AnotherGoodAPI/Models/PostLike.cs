using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.Models
{
    public class PostLike
    {
        public int Id { get; set; }

        // Relations
        [Required]
        public int PostId { get; set; }
        public Post Post { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
