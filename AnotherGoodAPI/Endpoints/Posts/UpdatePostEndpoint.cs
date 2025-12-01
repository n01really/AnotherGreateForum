using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Posts;

public class UpdatePostEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/posts/{id:int}", HandleAsync)
           .WithName("UpdatePost")
           .Produces(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status403Forbidden)
           .Produces(StatusCodes.Status404NotFound);
    }

    public record Request(string Title, string Content, int CategoryId);

    public async Task<IResult> HandleAsync(int id, Request request, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null)
            return Results.Unauthorized();

        var post = await db.Posts.FindAsync(id);
        if (post == null)
            return Results.NotFound();

        if (post.AuthorId != userId)
            return Results.Forbid();

        post.Title = request.Title;
        post.Body = request.Content;
        post.CategoryId = request.CategoryId;

        await db.SaveChangesAsync();
        return Results.Ok(post);
    }
}
