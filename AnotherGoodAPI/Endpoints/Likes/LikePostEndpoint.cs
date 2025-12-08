using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AnotherGoodAPI.Endpoints.Likes;

public class LikePostEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/likes/post/{postId:int}", HandleAsync)
           .WithName("LikePost")
           .Produces<Response>(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record Response(int Id, int PostId, string UserId);

    public async Task<IResult> HandleAsync(int postId, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Results.Unauthorized();

        var postExists = await db.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists)
            return Results.BadRequest("Post does not exist.");

        var alreadyLiked = await db.PostLikes.AnyAsync(pl => pl.PostId == postId && pl.UserId == userId);
        if (alreadyLiked)
            return Results.BadRequest("You already liked this post.");

        var like = new PostLike
        {
            PostId = postId,
            UserId = userId
        };

        db.PostLikes.Add(like);
        await db.SaveChangesAsync();

        var response = new Response(like.Id, like.PostId, like.UserId);
        return Results.Created($"/likes/{like.Id}", response);
    }
}
