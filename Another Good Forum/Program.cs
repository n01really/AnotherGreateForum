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

            // Configure HttpClient for API calls
            builder.Services.AddHttpClient<Pages.RegisterModel>();
            builder.Services.AddHttpClient<Pages.LoginModel>();
            builder.Services.AddHttpClient<Pages.AdminPageModel>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7242");
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
