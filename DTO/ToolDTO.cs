using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace RecipeManagementBE.DTO {
    public class ToolDTO {
        public long Id { get; set; }

        [StringLength(128)] public string Name { get; set; }
    }
}