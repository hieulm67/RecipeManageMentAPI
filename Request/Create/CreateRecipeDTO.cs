using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RecipeManagementBE.DTO;

namespace RecipeManagementBE.Request.Create {
    public class CreateRecipeDTO {
        
        public string Description { get; set; }

        [Required]
        public string ImageDescription { get; set; }

        [Required]
        public long DishId { get; set; }

        [JsonIgnore]
        public bool IsUsing { get; set; }

        public HashSet<ProcessingStepDTO> ProcessingSteps { get; set; }

        public HashSet<RecipeDetailDTO> RecipeDetails { get; set; }

        public HashSet<RecipeToolDTO> RecipeTools { get; set; }
    }
}