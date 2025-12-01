using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Messages;

public class MarkAsReadEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/messages/{messageId}/read", HandleAsync)
           .WithName("MarkAsRead")
           .Produces(StatusCodes.Status200OK)
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

        if (message.ReceiverId != userId) return Results.Forbid();

        message.IsRead = true;
        await db.SaveChangesAsync();

        return Results.Ok(message);
    }
}
