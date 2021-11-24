using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

#nullable disable

namespace RecipeManagementBE.DTO {
    public class DishDTO {
        public long Id { get; set; }

        [StringLength(128)] public string Name { get; set; }

        public string Description { get; set; }

        [JsonPropertyName("created_by")]
        public EmployeeDTO Manager { get; set; }

        public long CategoryId { get; set; }

        public string CategoryName { get; set; }

        public ICollection<RecipeDTO> Recipes { get; set; }
    }
}