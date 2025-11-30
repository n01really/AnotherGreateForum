using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Messages;

public class SendMessageEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/messages", HandleAsync)
           .WithName("SendMessage")
           .Produces(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record Request(string ReceiverId, string Body, int? ParentMessageId = null);
    public record Response(DirectMessage Message);

    public async Task<IResult> HandleAsync(Request request, ForumDbContext db, HttpContext http)
    {
        var senderId = http.User.Identity?.Name;
        if (senderId == null) return Results.Unauthorized();

        var receiverExists = await db.Users.AnyAsync(u => u.Id == request.ReceiverId);
        if (!receiverExists) return Results.BadRequest("Receiver does not exist.");

        var message = new DirectMessage
        {
            Body = request.Body,
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
            ParentMessageId = request.ParentMessageId
        };

        db.DirectMessages.Add(message);
        await db.SaveChangesAsync();

        return Results.Created($"/messages/{message.Id}", new Response(message));
    }
}
