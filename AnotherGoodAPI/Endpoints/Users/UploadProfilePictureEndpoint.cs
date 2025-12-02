using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AnotherGoodAPI.Endpoints.Users;

public class UploadProfilePictureEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/users/profile-picture", HandleAsync)
           .WithName("UploadProfilePicture")
           .Produces<Response>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status404NotFound)
           .Produces(StatusCodes.Status400BadRequest)
           .Accepts<IFormFile>("multipart/form-data");
    }

    public record Response(string ProfilePictureUrl);

    public async Task<IResult> HandleAsync(IFormFile file, UserManager<ApplicationUser> userManager, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null)
            return Results.Unauthorized();

        var user = await userManager.FindByNameAsync(userId);
        if (user == null)
            return Results.NotFound();

        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded.");

        // Example storage logic: store only file name; replace with real storage if needed
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        user.ProfilePictureUrl = fileName;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return Results.BadRequest(errors);
        }

        return Results.Ok(new Response(user.ProfilePictureUrl));
    }
}
