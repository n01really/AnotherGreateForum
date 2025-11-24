namespace AnotherGoodAPI.Endpoints.Forum
{
    public class CreateForum
    {
        public static Task<ForumDto> HandleAsync(ForumDto request)
        {
            request.Id = ForumStore.Forums.Count + 1;
            ForumStore.Forums.Add(request);
            return Task.FromResult(request);
        }
    }
}
