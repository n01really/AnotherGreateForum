using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AnotherGoodAPI.Endpoints.Users;

public class LogoutUserEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/users/logout", HandleAsync)
           .WithName("LogoutUser")
           .Produces(StatusCodes.Status200OK);
    }

    public record Response(string Message);

    public async Task<IResult> HandleAsync(SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.Ok(new Response("Logged out successfully"));
    }
}
