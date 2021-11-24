using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeManagementBE.Request.Create {
    public class RegisterDTO {
        
        [StringLength(64)]
        [Required]
        [JsonPropertyName("uid")]
        public string UID { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 6)]
        public string Password { get; set; }

        [EmailAddress]
        [StringLength(256)]
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(128)]
        public string FullName { get; set; }

        [Phone]
        [StringLength(15)]
        public string Phone { get; set; }

        public bool IsManager { get; set; }

        public long BrandId { get; set; }
    }
}