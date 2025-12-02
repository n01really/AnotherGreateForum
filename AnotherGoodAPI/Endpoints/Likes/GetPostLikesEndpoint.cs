using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Likes;

public class GetPostLikesEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/likes/post/{postId:int}", HandleAsync)
           .WithName("GetPostLikes")
           .Produces<Response>(StatusCodes.Status200OK);
    }

    public record Response(int PostId, int Likes);

    public async Task<IResult> HandleAsync(int postId, ForumDbContext db)
    {
        var likeCount = await db.PostLikes.CountAsync(pl => pl.PostId == postId);
        return Results.Ok(new Response(postId, likeCount));
    }
}
