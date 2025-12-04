using Another_Great_Forum.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Another_Great_Forum
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Get API base URL from configuration
            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7286";

            // Configure named HttpClients
            builder.Services.AddHttpClient(nameof(Pages.RegisterModel), client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });
            
            builder.Services.AddHttpClient(nameof(Pages.LoginModel), client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });
            
            builder.Services.AddHttpClient(nameof(Pages.AdminPageModel), client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages();

            app.Run();
        }
    }
}
