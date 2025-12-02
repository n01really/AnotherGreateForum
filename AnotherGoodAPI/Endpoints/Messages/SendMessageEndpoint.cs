using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.Endpoints.Messages;

public class SendMessageEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/messages", HandleAsync)
           .WithName("SendMessage")
           .Produces<Response>(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record Request(
        [Required] string ReceiverId,
        [Required] string Body,
        int? ParentMessageId = null
    );

    public record Response(int Id, string Body, string SenderId, string ReceiverId, DateTime SentAt, bool IsRead, int? ParentMessageId);

    public async Task<IResult> HandleAsync(Request request, ForumDbContext db, HttpContext http)
    {
        var senderId = http.User.Identity?.Name;
        if (senderId == null)
            return Results.Unauthorized();

        var receiverExists = await db.Users.AnyAsync(u => u.Id == request.ReceiverId);
        if (!receiverExists)
            return Results.BadRequest("Receiver does not exist.");

        var message = new DirectMessage
        {
            Body = request.Body,
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
            ParentMessageId = request.ParentMessageId
        };

        db.DirectMessages.Add(message);
        await db.SaveChangesAsync();

        var response = new Response(message.Id, message.Body, message.SenderId, message.ReceiverId, message.SentAt, message.IsRead, message.ParentMessageId);
        return Results.Created($"/messages/{message.Id}", response);
    }
}
