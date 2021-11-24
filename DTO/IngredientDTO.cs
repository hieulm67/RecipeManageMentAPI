using System.ComponentModel.DataAnnotations;

#nullable disable

namespace RecipeManagementBE.DTO {
    public class IngredientDTO {
        public long Id { get; set; }

        [StringLength(128)] public string Name { get; set; }
    }
}