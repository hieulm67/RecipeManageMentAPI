using System.Collections.Generic;

#nullable disable

namespace RecipeManagementBE.DTO {
    public class RecipeDTO {
        public long Id { get; set; }

        public string Description { get; set; }

        public string ImageDescription { get; set; }

        public long DishId { get; set; }

        public bool IsUsing { get; set; }

        public HashSet<ProcessingStepDTO> ProcessingSteps { get; set; }

        public List<QaDTO> Qas { get; set; }

        public HashSet<RecipeDetailDTO> RecipeDetails { get; set; }

        public HashSet<RecipeToolDTO> RecipeTools { get; set; }
    }
}