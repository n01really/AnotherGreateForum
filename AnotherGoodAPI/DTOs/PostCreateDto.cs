using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.DTOs
{
    public class PostCreateDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string AuthorId { get; set; }
    }
}
