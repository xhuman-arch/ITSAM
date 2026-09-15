using ITServiceAssetManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace ITServiceAssetManagement.Data
{
    public static class DbSeeder
    {
        public static readonly string[] Roles = new[]
        {
            "EMPLOYEE",
            "IT_SERVICE",
            "IT_SUPPORT",
            "PURCHASING"
        };

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Contoh user admin awal (IT_SERVICE) - opsional, bisa dihapus/ubah sesuai kebutuhan
            var adminEmail = "admin@itsam.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@12345");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "IT_SERVICE");
                }
            }
        }
    }
}
