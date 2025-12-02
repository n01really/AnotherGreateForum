using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.Endpoints.Users;

public class RegisterUserEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/users/register", HandleAsync)
           .WithTags("Users")
           .Produces<Response>(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest);
    }

    public record Request(
        [Required] string DisplayName,
        [Required, EmailAddress] string Email,
        [Required, MinLength(6)] string Password
    );

    public record Response(string Id, string DisplayName, string Email, string? ProfilePictureUrl);

    private async Task<IResult> HandleAsync(Request request, UserManager<ApplicationUser> userManager)
    {
        // Check if Email already exists
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return Results.BadRequest("A user with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return Results.BadRequest(errors);
        }

        var response = new Response(
            user.Id,
            user.DisplayName!,
            user.Email!,
            user.ProfilePictureUrl
        );

        return Results.Created($"/users/{user.Id}", response);
    }
}
