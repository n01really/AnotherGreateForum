using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI.Endpoints.Categories;

public class CreateCategoryEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/categories", HandleAsync)
           .WithName("CreateCategory")
           .Produces(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest);
    }

    public async Task<IResult> HandleAsync(Category request, ForumDbContext db)
    {
        var exists = await db.Categories.AnyAsync(c => c.Name == request.Name);
        if (exists) return Results.BadRequest("Category already exists.");

        db.Categories.Add(request);
        await db.SaveChangesAsync();

        return Results.Created($"/categories/{request.Id}", request);
    }
}
