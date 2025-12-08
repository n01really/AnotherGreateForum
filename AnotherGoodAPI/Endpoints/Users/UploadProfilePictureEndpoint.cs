using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
           .Produces(StatusCodes.Status400BadRequest)
           .DisableAntiforgery();
    }

    public record Response(string ProfilePictureUrl);

    public async Task<IResult> HandleAsync(
        [FromForm(Name = "file")] IFormFile file,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment env,
        HttpContext http)
    {
        var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Results.Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Results.NotFound();

        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded.");

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png" }; // "image/jpg" is not needed
        if (!allowedTypes.Contains(file.ContentType))
            return Results.BadRequest("Only JPG & PNG images are allowed.");

        // ✅ Ensure WebRootPath is set
        if (string.IsNullOrEmpty(env.WebRootPath))
            return Results.Problem("Web root path is not configured.");

        string uploadFolder = Path.Combine(env.WebRootPath, "profile-pictures");

        // Ensure folder exists
        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        string fileName = $"{user.Id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        string filePath = Path.Combine(uploadFolder, fileName);

        // Save file
        using (var stream = File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        // Update user's profile picture URL
        user.ProfilePictureUrl = $"/profile-pictures/{fileName}";
        await userManager.UpdateAsync(user);

        return Results.Ok(new { profilePictureUrl = user.ProfilePictureUrl });
    }
}
