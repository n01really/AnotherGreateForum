using AnotherGoodAPI.Models;
using AnotherGoodAPI.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AnotherGoodAPI.Endpoints.Users;

public class RegisterUserEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/users/register", Handle)
           .WithTags("Users")
           .Produces<UserDto>(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest);
    }

    public record Request(string DisplayName, string Email, string Password);
    public record Response(string Id, string DisplayName, string Email, string? ProfilePictureUrl);

    private async Task<IResult> Handle(Request request, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        if (request == null)
            return Results.BadRequest("Request body is empty.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Results.BadRequest(result.Errors);

        await signInManager.SignInAsync(user, isPersistent: false);

        var response = new Response(user.Id, user.DisplayName, user.Email, user.ProfilePictureUrl);
        return Results.Created($"/users/{user.Id}", response);
    }
}
