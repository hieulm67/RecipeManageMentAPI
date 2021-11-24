using System.ComponentModel.DataAnnotations;

#nullable disable

namespace RecipeManagementBE.DTO {
    public class BrandDTO {
        public long Id { get; set; }

        [Required]
        [StringLength(128)] public string Name { get; set; }

        public string Logo { get; set; }

        [Required]
        [Phone]
        [StringLength(15)] public string Phone { get; set; }
        
        public string Address { get; set; }

        [StringLength(256)] [EmailAddress] public string Email { get; set; }

        public string HomePage { get; set; }
    }
}