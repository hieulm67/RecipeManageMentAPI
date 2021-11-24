using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace RecipeManagementBE.DTO {
    public class CategoryDTO {
        
        public long Id { get; set; }

        [Required]
        [StringLength(128)] public string Name { get; set; }

        public string Description { get; set; }

        public long BrandId { get; set; }

        public ICollection<DishDTO> Dishes { get; set; }
    }
}