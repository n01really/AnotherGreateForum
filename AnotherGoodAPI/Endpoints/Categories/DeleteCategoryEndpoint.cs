using AnotherGoodAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AnotherGoodAPI.Endpoints.Categories;

public class DeleteCategoryEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/categories/{id:int}", HandleAsync)
           .WithName("DeleteCategory")
           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status404NotFound);
    }

    public async Task<IResult> HandleAsync(int id, ForumDbContext db)
    {
        var category = await db.Categories.FindAsync(id);
        if (category == null)
            return Results.NotFound($"Category with id {id} was not found.");

        db.Categories.Remove(category);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
