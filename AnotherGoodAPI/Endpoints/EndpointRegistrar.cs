using AnotherGoodAPI.Endpoints.Categories;
﻿using AnotherGoodAPI.Endpoints.Posts;
using AnotherGoodAPI.Endpoints.Users;
using AnotherGoodAPI.Endpoints.Comments;
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

            // Posts
            new CreatePostEndpoint().MapEndpoint(app);
            new UpdatePostEndpoint().MapEndpoint(app);
            new DeletePostEndpoint().MapEndpoint(app);
            new GetPostsEndpoint().MapEndpoint(app);
            new GetPostEndpoint().MapEndpoint(app);
            new ToggleLikeEndpoint().MapEndpoint(app);

            // Comments
            new CreateCommentEndpoint().MapEndpoint(app);
            new UpdateCommentEndpoint().MapEndpoint(app);
            new DeleteCommentEndpoint().MapEndpoint(app);
            new GetCommentsEndpoint().MapEndpoint(app);
        }
    }
}
