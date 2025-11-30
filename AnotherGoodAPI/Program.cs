using AnotherGoodAPI.Data;
using AnotherGoodAPI.Endpoints;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext
            builder.Services.AddDbContext<ForumDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ForumDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Middleware
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Map endpoints
            EndpointRegistrar.MapAllEndpoints(app);

            // Seed Identity Users and Roles
            await IdentitySeeder.SeedAsync(app.Services);

            app.Run();
        }
    }
}
