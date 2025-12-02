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
           .Accepts<IFormFile>("multipart/form-data")
           .Produces<Response>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status404NotFound)
           .Produces(StatusCodes.Status400BadRequest);
    }

    public record Response(string ProfilePictureUrl);

    public async Task<IResult> HandleAsync(
        IFormFile file,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment env,
        HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null) return Results.Unauthorized();

        var user = await userManager.FindByNameAsync(userId);
        if (user == null) return Results.NotFound();

        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded.");

        // Optional: Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png" };
        if (!allowedTypes.Contains(file.ContentType))
            return Results.BadRequest("Only JPG & PNG images are allowed.");

        // Create directory if missing
        string uploadFolder = Path.Combine(env.WebRootPath, "profile-pictures");
        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        // Create unique filename
        string fileName = $"{user.Id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        string filePath = Path.Combine(uploadFolder, fileName);

        // Save to server
        using (var stream = File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        // Save the URL path to database
        user.ProfilePictureUrl = $"/profile-pictures/{fileName}";
        await userManager.UpdateAsync(user);

        return Results.Ok(new Response(user.ProfilePictureUrl));
    }
}
