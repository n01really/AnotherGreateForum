using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.Endpoints.Users;

public class LoginUserEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/users/login", HandleAsync)
           .WithName("LoginUser")
           .Produces<Response>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status400BadRequest);
    }

    public record Request(
        [Required, EmailAddress] string Email,
        [Required] string Password
    );

    public record Response(string Message);

    public async Task<IResult> HandleAsync(Request request, SignInManager<ApplicationUser> signInManager)
    {
        if (request == null)
            return Results.BadRequest("Request body is empty.");

        var result = await signInManager.PasswordSignInAsync(
            request.Email,      // Use Email consistently
            request.Password,
            isPersistent: true,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
            return Results.Unauthorized();

        // Cookie is automatically set in response by Identity
        return Results.Ok(new Response("Logged in successfully"));
    }

}
