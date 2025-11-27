using AnotherGoodAPI.Endpoints.Categories;
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

            // Categories
            new CreateCategoryEndpoint().MapEndpoint(app);
            new UpdateCategoryEndpoint().MapEndpoint(app);
            new DeleteCategoryEndpoint().MapEndpoint(app);
            new GetCategoriesEndpoint().MapEndpoint(app);

        }
    }
}
