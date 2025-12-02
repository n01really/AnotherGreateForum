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
           .Produces<Response>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status404NotFound);
    }

    public record Response(
        int Id,
        string Title,
        string Body,
        string AuthorName,
        string CategoryName,
        DateTime CreatedAt,
        int CommentCount,
        int LikeCount
    );

    public async Task<IResult> HandleAsync(int id, ForumDbContext db)
    {
        var post = await db.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return Results.NotFound();

        var response = new Response(
            post.Id,
            post.Title,
            post.Body,
            post.Author?.DisplayName ?? "Unknown",
            post.Category?.Name ?? "Unknown",
            post.CreatedAt,
            post.Comments.Count,
            post.Likes.Count
        );

        return Results.Ok(response);
    }
}
