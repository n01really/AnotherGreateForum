using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AnotherGoodAPI.Endpoints.Posts;

public class DeletePostEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/posts/{id:int}", HandleAsync)
           .WithName("DeletePost")
           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status403Forbidden)
           .Produces(StatusCodes.Status404NotFound);
    }

    public async Task<IResult> HandleAsync(int id, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = http.User.IsInRole("Admin");

        var post = await db.Posts.FindAsync(id);
        if (post == null)
            return Results.NotFound();

        if (post.AuthorId != userId && !isAdmin)
            return Results.Forbid();

        db.Posts.Remove(post);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
