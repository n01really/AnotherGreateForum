using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Comments;

public class GetCommentsEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/comments/post/{postId:int}", HandleAsync)
           .WithName("GetComments")
           .Produces(StatusCodes.Status200OK);
    }

    public async Task<IResult> HandleAsync(int postId, ForumDbContext db)
    {
        var comments = await db.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.Author)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return Results.Ok(comments);
    }
}
