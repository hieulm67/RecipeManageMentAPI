using System.ComponentModel.DataAnnotations;

namespace RecipeManagementBE.Request.Authentication {
    public class TokenRequest {
        
        [Required]
        public string Token { get; set; }
        
        [Required]
        public string RefreshToken { get; set; }
    }
}