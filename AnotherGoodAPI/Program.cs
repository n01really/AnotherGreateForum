
using AnotherGoodAPI.Endpoints.Forum;

namespace AnotherGoodAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            // Minimal API endpoints for Forum (in-memory)
            app.MapPost("/forums", CreateForum.HandleAsync);
            app.MapGet("/forums", GetForum.HandleAsync);

            app.Run();
        }
    }
}
