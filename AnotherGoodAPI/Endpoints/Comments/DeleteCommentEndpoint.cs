using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AnotherGoodAPI.Endpoints.Comments;

public class DeleteCommentEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/comments/{id:int}", HandleAsync)
           .WithName("DeleteComment")
           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status403Forbidden)
           .Produces(StatusCodes.Status404NotFound);
    }

    public async Task<IResult> HandleAsync(int id, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null) return Results.Unauthorized();

        var comment = await db.Comments.FindAsync(id);
        if (comment == null) return Results.NotFound();

        if (comment.AuthorId != userId) return Results.Forbid();

        db.Comments.Remove(comment);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
