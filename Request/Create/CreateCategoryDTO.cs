using System.ComponentModel.DataAnnotations;

namespace RecipeManagementBE.Request.Create {
    public class CreateCategoryDTO {

        [StringLength(128)]
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}