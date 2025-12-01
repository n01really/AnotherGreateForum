using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Comments;

public class CreateCommentEndpoint : IEndpointMapper
{
    public record Request(int PostId, string Body);

    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/comments", HandleAsync)
           .WithName("CreateComment")
           .Produces(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    public async Task<IResult> HandleAsync(Request request, ForumDbContext db, HttpContext http)
    {
        var userId = http.User.Identity?.Name;
        if (userId == null) return Results.Unauthorized();

        var postExists = await db.Posts.AnyAsync(p => p.Id == request.PostId);
        if (!postExists) return Results.BadRequest("Post does not exist.");

        var comment = new Comment
        {
            Body = request.Body,
            PostId = request.PostId,
            AuthorId = userId
        };

        db.Comments.Add(comment);
        await db.SaveChangesAsync();

        return Results.Created($"/comments/{comment.Id}", comment);
    }
}
