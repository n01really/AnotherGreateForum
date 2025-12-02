using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using AnotherGoodAPI.Models;

namespace AnotherGoodAPI.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Roles
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin Seed
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    DisplayName = "Admin Master"
                };
                var password = "Admin123!";

                await userManager.CreateAsync(adminUser, password);
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // User 1 Seed
            var user1Email = "user1@example.com";
            if (await userManager.FindByEmailAsync(user1Email) == null)
            {
                var user1 = new ApplicationUser
                {
                    UserName = user1Email,
                    Email = user1Email,
                    DisplayName = "First User"
                };
                var password = "Password1!";

                await userManager.CreateAsync(user1, password);
                await userManager.AddToRoleAsync(user1, "User");
            }

            // User 2 Seed
            var user2Email = "user2@example.com";
            if (await userManager.FindByEmailAsync(user2Email) == null)
            {
                var user2 = new ApplicationUser
                {
                    UserName = user2Email,
                    Email = user2Email,
                    DisplayName = "Second User"
                };
                var password = "Password1!";

                await userManager.CreateAsync(user2, password);
                await userManager.AddToRoleAsync(user2, "User");
            }
        }
    }
}
