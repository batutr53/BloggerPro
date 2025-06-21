using BloggerPro.Domain.Constants;
using BloggerPro.Domain.Entities;
using BloggerPro.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BloggerPro.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // Rolleri oluþtur
            foreach (var roleName in new[] { UserRoles.Admin, UserRoles.Editor, UserRoles.User })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new Role { Name = roleName });
            }

            // Admin kullanýcýyý oluþtur
            var adminEmail = "admin@bloggerpro.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
            }

            // Editor kullanýcýyý oluþtur
            var editorEmail = "editor@bloggerpro.com";
            var editorUser = await userManager.FindByEmailAsync(editorEmail);
            if (editorUser == null)
            {
                editorUser = new User
                {
                    UserName = "editor",
                    Email = editorEmail,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(editorUser, "Editor@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(editorUser, UserRoles.Editor);
            }
        }
    }

}
