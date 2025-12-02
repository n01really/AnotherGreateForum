using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Messages;

public class DeleteMessageEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/messages/{messageId}", HandleAsync)
           .WithName("DeleteMessage")
           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status403Forbidden)
           .Produces(StatusCodes.Status404NotFound);
    }

    public async Task<IResult> HandleAsync(int messageId, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null) return Results.Unauthorized();

        var message = await db.DirectMessages.FindAsync(messageId);
        if (message == null) return Results.NotFound();

        if (message.SenderId != userId && message.ReceiverId != userId)
            return Results.Forbid();

        db.DirectMessages.Remove(message);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
