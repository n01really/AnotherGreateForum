using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Likes;

public class LikePostEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/likes/post/{postId:int}", HandleAsync)
           .WithName("LikePost")
           .Produces(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public async Task<IResult> HandleAsync(int postId, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null) return Results.Unauthorized();

        var postExists = await db.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists) return Results.BadRequest("Post does not exist.");

        var alreadyLiked = await db.PostLikes.AnyAsync(pl => pl.PostId == postId && pl.UserId == userId);
        if (alreadyLiked) return Results.BadRequest("You already liked this post.");

        var like = new PostLike
        {
            PostId = postId,
            UserId = userId
        };

        db.PostLikes.Add(like);
        await db.SaveChangesAsync();

        return Results.Created($"/likes/{like.Id}", like);
    }
}
