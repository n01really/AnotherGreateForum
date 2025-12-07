using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AnotherGoodAPI.Endpoints.Messages;

public class GetChatUsersEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/messages/users", HandleAsync)
           .WithName("GetChatUsers")
           .Produces<List<UserResponse>>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record UserResponse(string Id, string DisplayName);

    public async Task<IResult> HandleAsync(ForumDbContext db, HttpContext http)
    {
        var currentUserId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            return Results.Unauthorized();

        // Get all distinct users you have messages with
        var userIds = await db.DirectMessages
            .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
            .Select(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
            .Distinct()
            .ToListAsync();

        var users = await db.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new UserResponse(u.Id, u.DisplayName))
            .ToListAsync();

        return Results.Ok(users);
    }
}
