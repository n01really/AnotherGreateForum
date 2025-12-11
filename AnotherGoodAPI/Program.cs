using AnotherGoodAPI.Data;
using AnotherGoodAPI.Endpoints;
using AnotherGoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace AnotherGoodAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);
            builder.Environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Conditionally configure database based on environment
            if (builder.Environment.IsEnvironment("Testing"))
            {
                // Use in-memory database for testing
                builder.Services.AddDbContext<ForumDbContext>(options =>
                    options.UseInMemoryDatabase("TestDatabase"));
            }
            else
            {
                // Use PostgreSQL for production and development
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                builder.Services.AddDbContext<ForumDbContext>(options =>
                    options.UseNpgsql(connectionString));
            }

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ForumDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;

                if (builder.Environment.IsEnvironment("Testing"))
                {
                    // For testing environment, disable redirects and use lax cookie settings
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    };
                }
                else
                {
                    // For production/development, use secure cookie settings
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                }
            });

            builder.Services.AddAuthorization();

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

            EndpointRegistrar.MapAllEndpoints(app);

            app.Run();
        }
    }
}
