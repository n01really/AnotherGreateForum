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

    public record CategoryDto(int Id, string Name, string Description);

    public async Task<IResult> HandleAsync(ForumDbContext db)
    {
        var categories = await db.Categories
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description))
            .ToListAsync();
        return Results.Ok(categories);
    }


}
