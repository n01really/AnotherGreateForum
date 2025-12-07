using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Likes;

public class UnlikePostEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/likes/post/{postId:int}", HandleAsync)
           .WithName("UnlikePost")
           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status404NotFound);
    }

    public async Task<IResult> HandleAsync(int postId, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null)
            return Results.Unauthorized();

        var like = await db.PostLikes.FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);
        if (like == null)
            return Results.NotFound();

        db.PostLikes.Remove(like);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
