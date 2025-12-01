using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Comments;

public class UpdateCommentEndpoint : IEndpointMapper
{
    public record Request(string Body);

    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/comments/{id:int}", HandleAsync)
           .WithName("UpdateComment")
           .Produces(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status403Forbidden)
           .Produces(StatusCodes.Status404NotFound);
    }

    public async Task<IResult> HandleAsync(int id, Request request, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null) return Results.Unauthorized();

        var comment = await db.Comments.FindAsync(id);
        if (comment == null) return Results.NotFound();

        if (comment.AuthorId != userId) return Results.Forbid();

        comment.Body = request.Body;
        comment.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Results.Ok(comment);
    }
}
