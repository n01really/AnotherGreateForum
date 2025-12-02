namespace AnotherGoodAPI.DTOs
{
    public class PostLikeDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
