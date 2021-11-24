using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace RecipeManagementBE.DTO {
    public class AccountDTO {
        
        [JsonPropertyName("uid")]
        [Required] [StringLength(64)] public string UID { get; set; }

        [StringLength(64, MinimumLength = 6)] public string Password { get; set; }

        [Required] [EmailAddress] [StringLength(256)] public string Email { get; set; }

        [StringLength(128)] public string FullName { get; set; }

        [Phone] [StringLength(15)] public string Phone { get; set; }
        
        public RoleDTO Role { get; set; }
    }
}