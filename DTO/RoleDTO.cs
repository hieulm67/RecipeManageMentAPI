using System.ComponentModel.DataAnnotations;

namespace RecipeManagementBE.DTO {
    public class RoleDTO {
        public long Id { get; set; }

        [StringLength(64)] public string Code { get; set; }

        [StringLength(128)] public string Name { get; set; }
    }
}