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

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            builder.Services.AddDbContext<ForumDbContext>(options =>
                options.UseNpgsql(connectionString));

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

            var frontendUrl1 = Environment.GetEnvironmentVariable("FRONTEND_URL_1");
            var frontendUrl2 = Environment.GetEnvironmentVariable("FRONTEND_URL_2");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.WithOrigins(frontendUrl1, frontendUrl2)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
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
