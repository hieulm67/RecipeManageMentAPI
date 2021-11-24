using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeManagementBE.DTO {
    public class RecipeToolDTO {
        public long RecipeId { get; set; }
        
        [Required]
        public long ToolId { get; set; }

        public ToolDTO Tool { get; set; }

        [StringLength(256)] public string Amount { get; set; }
        
        public override bool Equals(object? obj) {
            return GetHashCode() == obj?.GetHashCode();
        }

        public override int GetHashCode() {
            return Convert.ToInt32(ToolId);
        }
    }
}