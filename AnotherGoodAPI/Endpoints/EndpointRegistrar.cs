using AnotherGoodAPI.Endpoints.Posts;
using AnotherGoodAPI.Endpoints.Users;
using Microsoft.AspNetCore.Builder;

namespace AnotherGoodAPI.Endpoints
{
    public static class EndpointRegistrar
    {
        public static void MapAllEndpoints(WebApplication app)
        {
            // Users
            new RegisterUserEndpoint().MapEndpoint(app);
            new LoginUserEndpoint().MapEndpoint(app);
            new LogoutUserEndpoint().MapEndpoint(app);
            new UploadProfilePictureEndpoint().MapEndpoint(app);


            // Posts
            new CreatePostEndpoint().MapEndpoint(app);
            new UpdatePostEndpoint().MapEndpoint(app);
            new DeletePostEndpoint().MapEndpoint(app);
            new GetPostsEndpoint().MapEndpoint(app);
            new GetPostEndpoint().MapEndpoint(app);
            new ToggleLikeEndpoint().MapEndpoint(app);
        }
    }
}
