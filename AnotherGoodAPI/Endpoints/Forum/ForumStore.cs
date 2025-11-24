namespace AnotherGoodAPI.Endpoints.Forum
{
    public static class ForumStore
    {
        public static List<ForumDto> Forums { get; } = new List<ForumDto>();
    }

    public class ForumDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
