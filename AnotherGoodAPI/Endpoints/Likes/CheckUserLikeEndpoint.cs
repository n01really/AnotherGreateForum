using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Likes;

public class CheckUserLikeEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/likes/post/{postId:int}/user", HandleAsync)
           .WithName("CheckUserLike")
           .Produces<Response>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record Response(int PostId, bool Liked);

    public async Task<IResult> HandleAsync(int postId, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null)
            return Results.Unauthorized();

        var liked = await db.PostLikes.AnyAsync(pl => pl.PostId == postId && pl.UserId == userId);
        return Results.Ok(new Response(postId, liked));
    }
}
