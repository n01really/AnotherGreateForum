using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.Endpoints.Categories;

public class UpdateCategoryEndpoint : IEndpointMapper
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/categories/{id:int}", HandleAsync)
           .WithName("UpdateCategory")
           .Produces(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status404NotFound);
    }

    public record CategoryUpdateRequest(
        [Required] string Name,
        string Description
    );

    public record CategoryResponse(int Id, string Name, string Description);

    public async Task<IResult> HandleAsync(int id, CategoryUpdateRequest request, ForumDbContext db)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Results.BadRequest("Name is required.");

        var category = await db.Categories.FindAsync(id);
        if (category == null)
            return Results.NotFound();

        // Update values
        category.Name = request.Name;
        category.Description = request.Description;

        await db.SaveChangesAsync();

        var response = new CategoryResponse(category.Id, category.Name, category.Description);

        return Results.Ok(response);
    }
}
