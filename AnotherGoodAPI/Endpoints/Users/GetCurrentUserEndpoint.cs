using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AnotherGoodAPI.Endpoints.Users;

public class GetCurrentUserEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/users/me", async (HttpContext http, UserManager<ApplicationUser> userManager) =>
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Results.Unauthorized();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return Results.NotFound();

            // Return email and profile picture
            return Results.Ok(new
            {
                id = user.Id,
                name = user.DisplayName,
                email = user.Email,
                profilePictureUrl = user.ProfilePictureUrl
            });
        })
        .WithName("CurrentUser")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
    }
}
