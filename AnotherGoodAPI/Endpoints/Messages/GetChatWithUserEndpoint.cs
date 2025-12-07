using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AnotherGoodAPI.Endpoints.Messages;

public class GetChatWithUserEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/messages/chat/{otherUserId}", HandleAsync)
           .WithName("GetChatWithUser")
           .Produces<List<Response>>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record Response(
        int Id,
        string Body,
        string SenderId,
        string ReceiverId,
        string SenderName,
        string ReceiverName,
        DateTime SentAt
    );

    public async Task<IResult> HandleAsync(string otherUserId, ForumDbContext db, HttpContext http)
    {
        var currentUserId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            return Results.Unauthorized();

        var messages = await db.DirectMessages
            .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                        (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        var response = messages.Select(m => new Response(
            m.Id,
            m.Body,
            m.SenderId,
            m.ReceiverId,
            m.Sender.DisplayName,
            m.Receiver.DisplayName,
            m.SentAt
        )).ToList();

        return Results.Ok(response);
    }
}
