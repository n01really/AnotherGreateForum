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
           .RequireAuthorization()
           .WithName("GetCurrentUser")
           .Produces<Response>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record Response(string Id, string DisplayName, string Email, string? ProfilePictureUrl, IList<string> Roles);

    public async Task<IResult> HandleAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user == null)
            return Results.Unauthorized();

        var roles = await userManager.GetRolesAsync(user);

        var response = new Response(
            user.Id,
            user.DisplayName,
            user.Email ?? "",
            user.ProfilePictureUrl,
            roles
        );

        return Results.Ok(response);
    }
}
