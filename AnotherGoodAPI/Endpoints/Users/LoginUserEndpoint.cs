using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AnotherGoodAPI.Endpoints.Users;

public class LoginUserEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/users/login", HandleAsync)
           .WithName("LoginUser")
           .Produces(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public record Request(string UserName, string Password);
    public record Response(string Message);

    public async Task<IResult> HandleAsync(Request request, SignInManager<ApplicationUser> signInManager)
    {
        var result = await signInManager.PasswordSignInAsync(
            request.UserName,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
            return Results.Unauthorized();

        return Results.Ok(new Response("Logged in successfully"));
    }
}
