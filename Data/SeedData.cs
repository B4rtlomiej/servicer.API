using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using servicer.API.Models;

namespace servicer.API.Data
{
    public class SeedData
    {
        public static void SeedAdminAndProductSpecificationsData(DataContext context)
        {
            if (!context.Users.Any())
            {
                var adminData = System.IO.File.ReadAllText("Data/AdminUserData.json");
                var definition = new { password = "" };
                var adminPassword = JsonConvert.DeserializeAnonymousType(adminData, definition);
                var admin = JsonConvert.DeserializeObject<User>(adminData);
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(adminPassword.password, out passwordHash, out passwordSalt);
                admin.PasswordHash = passwordHash;
                admin.PasswordSalt = passwordSalt;
                admin.Username = admin.Username.ToLower();
                context.Users.Add(admin);

                var productSpecificationsData = System.IO.File.ReadAllText("Data/ProductSpecificationsData.json");
                var productSpecifications = JsonConvert.DeserializeObject<List<ProductSpecification>>(productSpecificationsData);
                context.ProductSpecifications.AddRange(productSpecifications);

                context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}