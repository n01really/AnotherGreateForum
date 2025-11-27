namespace AnotherGoodAPI.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string AuthorName { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CommentCount { get; set; }
        public int LikeCount { get; set; }
    }
}
