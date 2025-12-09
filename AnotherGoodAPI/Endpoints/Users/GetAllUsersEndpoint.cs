using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Users
{
    public class GetAllUsersEndpoint : IEndpointMapper
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/users", HandleAsync)
               .WithName("GetAllUsers")
               .Produces(StatusCodes.Status200OK);
        }

        public record UserResponse(string Id, string DisplayName, string? Email, string? ProfilePictureUrl);

        public async Task<IResult> HandleAsync(ForumDbContext db)
        {
            var users = await db.Users
                .Select(u => new UserResponse(
                    u.Id,
                    u.DisplayName,
                    u.Email,
                    u.ProfilePictureUrl
                ))
                .ToListAsync();

            return Results.Ok(users);
        }
    }
}