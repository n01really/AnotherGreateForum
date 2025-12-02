using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.Endpoints.Comments;

public class CreateCommentEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/comments", HandleAsync)
           .WithName("CreateComment")
           .Produces<Response>(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record Request(
        [Required] int PostId,
        [Required, MaxLength(1000)] string Body
    );

    public record Response(int Id, int PostId, string Body, string AuthorId, DateTime CreatedAt);

    public async Task<IResult> HandleAsync(Request request, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null)
            return Results.Unauthorized();

        if (!ModelStateIsValid(request, out var errors))
            return Results.BadRequest(errors);

        var postExists = await db.Posts.AnyAsync(p => p.Id == request.PostId);
        if (!postExists)
            return Results.BadRequest("Post does not exist.");

        var comment = new Comment
        {
            Body = request.Body,
            PostId = request.PostId,
            AuthorId = userId
        };

        db.Comments.Add(comment);
        await db.SaveChangesAsync();

        var response = new Response(comment.Id, comment.PostId, comment.Body, comment.AuthorId, comment.CreatedAt);
        return Results.Created($"/comments/{comment.Id}", response);
    }

    private bool ModelStateIsValid(Request request, out List<string> errors)
    {
        errors = new List<string>();

        if (request.PostId <= 0)
            errors.Add("PostId must be greater than 0.");

        if (string.IsNullOrWhiteSpace(request.Body))
            errors.Add("Body is required.");
        else if (request.Body.Length > 1000)
            errors.Add("Body cannot exceed 1000 characters.");

        return errors.Count == 0;
    }
}
