using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.DTOs
{
    public class CommentCreateDto
    {
        [Required, MaxLength(1000)]
        public string Body { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public string AuthorId { get; set; }
    }
}
