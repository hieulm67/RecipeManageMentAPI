using Microsoft.AspNetCore.Identity;

namespace RecipeManagementBE.Security {
    public class BCryptPasswordHasher {
        
        public static string HashPassword(string password) {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        
        public static bool VerifyHashedPassword(string hashedPassword, string providedPassword) {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }
}