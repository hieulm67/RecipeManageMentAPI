using System.ComponentModel.DataAnnotations;

namespace RecipeManagementBE.DTO {
    public class AdminDTO {
        
        public long Id { get; set; } = 0;

        [Required]
        public AccountDTO Account { get; set; }
    }
}