using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.Endpoints.Posts;

public class CreatePostEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/posts", HandleAsync)
           .WithName("CreatePost")
           .Produces<Response>(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record Request(
        [Required, MaxLength(200)] string Title,
        [Required] string Content,
        [Required] int CategoryId
    );

    public record Response(int Id, string Title, string Content, int CategoryId, string AuthorId);

    public async Task<IResult> HandleAsync(Request request, ForumDbContext db, HttpContext http)
    {
        
        var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Results.Unauthorized();

        if (userId == null)
            return Results.Unauthorized();

        if (!ModelStateIsValid(request, out var validationErrors))
            return Results.BadRequest(validationErrors);

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

        var response = new Response(post.Id, post.Title, post.Body, post.CategoryId, post.AuthorId);
        return Results.Created($"/posts/{post.Id}", response);
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
