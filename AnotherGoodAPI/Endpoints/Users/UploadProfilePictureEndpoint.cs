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
           .Produces(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status404NotFound)
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

        // For simplicity, just store the file name. Replace with storage logic as needed.
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        user.ProfilePictureUrl = fileName;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Results.BadRequest(result.Errors);

        return Results.Ok(new Response(user.ProfilePictureUrl));
    }
}
