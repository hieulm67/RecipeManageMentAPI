using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeManagementBE.Request.Authentication {
    public class LoginDTO {

        [Required]
        [StringLength(64)]
        [JsonPropertyName("uid")]
        public string UID { get; set; }
        
        [Required]
        [StringLength(256)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(64)]
        public string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; }
    }
}