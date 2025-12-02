using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Messages;

public class GetInboxEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/messages/inbox/{userId}", HandleAsync)
           .WithName("GetInbox")
           .Produces<List<Response>>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status403Forbidden);
    }

    public record Response(int Id, string Body, string SenderId, string ReceiverId, DateTime SentAt, bool IsRead, int? ParentMessageId);

    public async Task<IResult> HandleAsync(string userId, ForumDbContext db, HttpContext http)
    {
        var currentUserId = http.User.Identity?.Name;
        if (currentUserId != userId) return Results.Forbid();

        var messages = await db.DirectMessages
            .Where(m => m.ReceiverId == userId)
            .Include(m => m.Sender)
            .Include(m => m.ParentMessage)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        var response = messages.Select(m => new Response(
            m.Id, m.Body, m.SenderId, m.ReceiverId, m.SentAt, m.IsRead, m.ParentMessageId
        )).ToList();

        return Results.Ok(response);
    }
}
