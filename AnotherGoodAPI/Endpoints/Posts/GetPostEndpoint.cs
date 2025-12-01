using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Posts;

public class GetPostEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/posts/{id:int}", HandleAsync)
           .WithName("GetPost")
           .Produces(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status404NotFound);
    }

    public async Task<IResult> HandleAsync(int id, ForumDbContext db)
    {
        var post = await db.Posts
            .Include(p => p.Author)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return Results.NotFound();

        return Results.Ok(post);
    }
}
