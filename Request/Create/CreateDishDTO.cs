using System.ComponentModel.DataAnnotations;

namespace RecipeManagementBE.Request.Create {
    public class CreateDishDTO {
        
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsShow { get; set; } = true;

        public long CategoryId { get; set; }
    }
}