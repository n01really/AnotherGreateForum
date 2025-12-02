using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.Endpoints.Comments;

public class UpdateCommentEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/comments/{id:int}", HandleAsync)
           .WithName("UpdateComment")
           .Produces<Response>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status403Forbidden)
           .Produces(StatusCodes.Status404NotFound);
    }

    public record Request(
        [Required, MaxLength(1000)] string Body
    );

    public record Response(int Id, int PostId, string Body, string AuthorId, DateTime CreatedAt, DateTime? UpdatedAt);

    public async Task<IResult> HandleAsync(int id, Request request, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null)
            return Results.Unauthorized();

        if (!ModelStateIsValid(request, out var errors))
            return Results.BadRequest(errors);

        var comment = await db.Comments.FindAsync(id);
        if (comment == null)
            return Results.NotFound();

        if (comment.AuthorId != userId)
            return Results.Forbid();

        comment.Body = request.Body;
        comment.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        var response = new Response(comment.Id, comment.PostId, comment.Body, comment.AuthorId, comment.CreatedAt, comment.UpdatedAt);
        return Results.Ok(response);
    }

    private bool ModelStateIsValid(Request request, out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Body))
            errors.Add("Body is required.");
        else if (request.Body.Length > 1000)
            errors.Add("Body cannot exceed 1000 characters.");

        return errors.Count == 0;
    }
}
