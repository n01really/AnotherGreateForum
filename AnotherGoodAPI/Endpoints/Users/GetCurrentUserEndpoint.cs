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
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier); // ✅ get actual user ID
            if (userId == null)
                return Results.Unauthorized();

            var user = await userManager.FindByIdAsync(userId);
            return Results.Ok(new { id = user.Id, name = user.DisplayName });
        })
        .WithName("CurrentUser")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
