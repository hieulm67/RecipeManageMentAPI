using System;

namespace RecipeManagementBE.Response {
    public class AuthenticationResult {
        public string Token { get; set; }
        
        public string RefreshToken { get; set; }
        
        public DateTime ExpiresTo { get; set; }
    }
}