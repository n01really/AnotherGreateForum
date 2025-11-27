using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Posts;

public class GetPostsEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/posts", HandleAsync)
           .WithName("GetPosts")
           .Produces(StatusCodes.Status200OK);
    }

    public async Task<IResult> HandleAsync(ForumDbContext db)
    {
        var posts = await db.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Results.Ok(posts);
    }
}
