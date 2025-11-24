namespace AnotherGoodAPI.Endpoints.Forum
{
    public class GetForum
    {
        public static Task<List<ForumDto>> HandleAsync()
        {
            return Task.FromResult(ForumStore.Forums);
        }
    }
}
