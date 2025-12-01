using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.DTOs
{
    public class CommentUpdateDto
    {
        [Required, MaxLength(1000)]
        public string Body { get; set; }
    }
}
