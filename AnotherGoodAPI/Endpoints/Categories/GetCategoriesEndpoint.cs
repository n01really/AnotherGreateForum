using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Categories;

public class GetCategoriesEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/categories", HandleAsync)
           .WithName("GetCategories")
           .Produces(StatusCodes.Status200OK);
    }

    public async Task<IResult> HandleAsync(ForumDbContext db)
    {
        var categories = await db.Categories.ToListAsync();
        return Results.Ok(categories);
    }
}
