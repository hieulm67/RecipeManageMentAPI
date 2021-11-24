
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace RecipeManagementBE.DTO {
    public partial class EmployeeDTO {
        
        public long Id { get; set; }

        public bool IsManager { get; set; } = false;

        public AccountDTO Account { get; set; }

        public BrandDTO Brand { get; set; }
    }
}