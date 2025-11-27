using AnotherGoodAPI.Data;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AnotherGoodAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddDbContext<ForumDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ForumDbContext>()
                .AddDefaultTokenProviders();


            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            var app = builder.Build();


            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.Run();
        }
    }
}
