using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Another_Great_Forum
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();

            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7286";

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

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapRazorPages();

            app.Run();
        }
    }
}