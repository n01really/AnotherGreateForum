using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

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
        app.MapGet("/users/current", HandleAsync)
           .RequireAuthorization()
           .WithName("GetCurrentUser")
           .Produces<Response>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record Response(string Id, string DisplayName, string Email, IList<string> Roles);

    public async Task<IResult> HandleAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.GetUserAsync(context.User);
        
        if (user == null)
            return Results.Unauthorized();

        var roles = await userManager.GetRolesAsync(user);
        var response = new Response(user.Id, user.DisplayName, user.Email ?? "", roles);
        
        return Results.Ok(response);
    }
}
