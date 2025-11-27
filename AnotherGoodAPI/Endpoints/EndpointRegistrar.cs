using Microsoft.AspNetCore.Builder;
using AnotherGoodAPI.Endpoints.Users;

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
        }
    }
}
