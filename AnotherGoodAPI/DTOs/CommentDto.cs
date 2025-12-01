namespace AnotherGoodAPI.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public string AuthorName { get; set; }
        public int PostId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
