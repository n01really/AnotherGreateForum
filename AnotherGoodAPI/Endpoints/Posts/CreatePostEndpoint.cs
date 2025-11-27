using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Posts;

public class CreatePostEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/posts", HandleAsync)
           .WithName("CreatePost")
           .Produces(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record Request(string Title, string Content, int CategoryId);
    public record Response(Post Post);

    public async Task<IResult> HandleAsync(Request request, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null)
            return Results.Unauthorized();

        var categoryExists = await db.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            return Results.BadRequest("Invalid category.");

        var post = new Post
        {
            Title = request.Title,
            Body = request.Content,
            CategoryId = request.CategoryId,
            AuthorId = userId
        };

        db.Posts.Add(post);
        await db.SaveChangesAsync();

        return Results.Created($"/posts/{post.Id}", new Response(post));
    }
}
