using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace RecipeManagementBE.DTO {
    public class RecipeDetailDTO {
        public long RecipeId { get; set; }
        
        [Required]
        [JsonPropertyName("ingredient_id")]
        public long IngredientId { get; set; }

        public IngredientDTO Ingredient { get; set; }

        [StringLength(256)] public string Amount { get; set; }

        public override bool Equals(object? obj) {
            return GetHashCode() == obj?.GetHashCode();
        }

        public override int GetHashCode() {
            return Convert.ToInt32(IngredientId);
        }
    }
}