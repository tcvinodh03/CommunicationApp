using CommunicationAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CommunicationAPI.Data
{
    public class Seed
    {
        public static async Task SeedUser(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            //var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            var objRoles = new List<AppRole>
            {
                new AppRole{Name="Member"},
                new AppRole{Name="Admin"},
                new AppRole{Name="Moderator"}
            };

            foreach (var role in objRoles)
            {
                await roleManager.CreateAsync(role);
            }
            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "P@$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");

                //using var hmac = new HMACSHA512();

                //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Password"));
                //user.PasswordSalt = hmac.Key;
                //context.Users.Add(user);
            }
            var admin = new AppUser
            {
                UserName = "admin"
            };
            await userManager.CreateAsync(admin, "P@$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });

            //await context.SaveChangesAsync();

        }
    }
}
