namespace AnotherGoodAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
