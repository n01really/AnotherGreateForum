using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Categories;

public class UpdateCategoryEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/categories/{id:int}", HandleAsync)
           .WithName("UpdateCategory")
           .Produces(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status404NotFound);
    }

    public async Task<IResult> HandleAsync(int id, Category request, ForumDbContext db)
    {
        var category = await db.Categories.FindAsync(id);
        if (category == null) return Results.NotFound();

        category.Name = request.Name;
        category.Description = request.Description;

        await db.SaveChangesAsync();
        return Results.Ok(category);
    }
}
