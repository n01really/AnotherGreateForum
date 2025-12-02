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
           .Produces<List<Response>>(StatusCodes.Status200OK);
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

    public async Task<IResult> HandleAsync(ForumDbContext db)
    {
        var posts = await db.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var response = posts.Select(p => new Response(
            p.Id,
            p.Title,
            p.Body,
            p.Author?.DisplayName ?? "Unknown",
            p.Category?.Name ?? "Unknown",
            p.CreatedAt,
            p.Comments.Count,
            p.Likes.Count
        )).ToList();

        return Results.Ok(response);
    }
}
