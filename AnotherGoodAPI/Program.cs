using AnotherGoodAPI.Data;
using AnotherGoodAPI.Endpoints;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DotNetEnv; // <-- add this

namespace AnotherGoodAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 1?? Load .env file
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);
            builder.Environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // 2?? Read connection string from .env
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            // 3?? Add DbContext
            builder.Services.AddDbContext<ForumDbContext>(options =>
                options.UseNpgsql(connectionString));

            // 4?? Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ForumDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.AddAuthorization();

            // 5?? CORS using .env
            var frontendUrl1 = Environment.GetEnvironmentVariable("FRONTEND_URL_1");
            var frontendUrl2 = Environment.GetEnvironmentVariable("FRONTEND_URL_2");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    var origins = new[] { frontendUrl1, frontendUrl2 }
                        .Where(url => !string.IsNullOrEmpty(url))
                        .ToArray();

                    if (origins.Length > 0)
                    {
                        policy.WithOrigins(origins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                    else
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    }
                });
            });

            var app = builder.Build();

            app.UseCors("FrontendPolicy");
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Map endpoints
            EndpointRegistrar.MapAllEndpoints(app);

            app.Run();
        }
    }
}
