using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
        var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Results.Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Results.NotFound();

        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded.");

        var uploadsFolder = Path.Combine("wwwroot", "profile-pictures");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        user.ProfilePictureUrl = $"/profile-pictures/{fileName}";
        await userManager.UpdateAsync(user);

        return Results.Ok(new Response(user.ProfilePictureUrl));
    }

}
