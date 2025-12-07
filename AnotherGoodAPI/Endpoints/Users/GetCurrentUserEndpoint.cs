using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AnotherGoodAPI.Endpoints.Users;

public class GetCurrentUserEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/users/current", HandleAsync)
           .RequireAuthorization() // must be logged in
           .WithName("GetCurrentUser")
           .Produces<Response>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    // Response including roles
    public record Response(string Id, string DisplayName, string Email, IList<string> Roles);

    // The actual handler
    public async Task<IResult> HandleAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
        // Get the current user from the HttpContext
        var user = await userManager.GetUserAsync(context.User);

        if (user == null)
            return Results.Unauthorized();

        // Get roles (e.g., Admin, User)
        var roles = await userManager.GetRolesAsync(user);

        // Build response
        var response = new Response(
            user.Id,
            user.DisplayName,
            user.Email ?? "",
            roles
        );

        return Results.Ok(response);
    }
}
