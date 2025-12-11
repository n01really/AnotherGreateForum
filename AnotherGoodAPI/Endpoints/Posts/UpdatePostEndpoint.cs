using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AnotherGoodAPI.Endpoints.Posts;

public class UpdatePostEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/posts/{id:int}", HandleAsync)
           .WithName("UpdatePost")
           .Produces<Response>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status403Forbidden)
           .Produces(StatusCodes.Status404NotFound);
    }

    public record Request(
        [Required, MaxLength(200)] string Title,
        [Required] string Content,
        [Required] int CategoryId
    );

    public record Response(int Id, string Title, string Content, int CategoryId, string AuthorId);

    public async Task<IResult> HandleAsync(int id, Request request, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Results.Unauthorized();

        if (!ModelStateIsValid(request, out var validationErrors))
            return Results.BadRequest(validationErrors);

        var post = await db.Posts.FindAsync(id);
        if (post == null)
            return Results.NotFound();

        if (post.AuthorId != userId)
            return Results.Forbid();

        post.Title = request.Title;
        post.Body = request.Content;
        post.CategoryId = request.CategoryId;
        post.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        var response = new Response(post.Id, post.Title, post.Body, post.CategoryId, post.AuthorId);
        return Results.Ok(response);
    }

    private bool ModelStateIsValid(Request request, out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Title is required.");
        if (request.Title?.Length > 200)
            errors.Add("Title cannot exceed 200 characters.");
        if (string.IsNullOrWhiteSpace(request.Content))
            errors.Add("Content is required.");
        if (request.CategoryId <= 0)
            errors.Add("CategoryId must be greater than 0.");

        return errors.Count == 0;
    }
}
