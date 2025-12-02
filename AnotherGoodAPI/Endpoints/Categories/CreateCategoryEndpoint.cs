using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

    public record CategoryCreateRequest(
        [Required] string Name,
        string Description
    );

    public record CategoryResponse(int Id, string Name, string Description);

    public async Task<IResult> HandleAsync(CategoryCreateRequest request, ForumDbContext db)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Name))
            return Results.BadRequest("Name is required.");

        // Check for duplicate
        var exists = await db.Categories.AnyAsync(c => c.Name == request.Name);
        if (exists)
            return Results.BadRequest("Category already exists.");

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var response = new CategoryResponse(category.Id, category.Name, category.Description);

        return Results.Created($"/categories/{category.Id}", response);
    }
}
