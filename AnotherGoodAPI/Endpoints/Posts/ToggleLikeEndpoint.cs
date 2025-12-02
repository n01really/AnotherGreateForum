using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Posts;

public class ToggleLikeEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/posts/{id:int}/togglelike", HandleAsync)
           .WithName("ToggleLike")
           .Produces<Response>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status404NotFound);
    }

    public record Response(string Message);

    public async Task<IResult> HandleAsync(int id, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null)
            return Results.Unauthorized();

        var post = await db.Posts.FindAsync(id);
        if (post == null)
            return Results.NotFound();

        var like = await db.PostLikes
            .FirstOrDefaultAsync(l => l.PostId == id && l.UserId == userId);

        string message;

        if (like != null)
        {
            db.PostLikes.Remove(like);
            message = "unliked";
        }
        else
        {
            db.PostLikes.Add(new PostLike { PostId = id, UserId = userId });
            message = "liked";
        }

        await db.SaveChangesAsync();
        return Results.Ok(new Response(message));
    }
}
