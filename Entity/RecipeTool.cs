using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("RecipeTool")]
    public class RecipeTool
    {
        [Key]
        public long Id { get; set; }
        public long RecipeId { get; set; }
        public long ToolId { get; set; }
        [StringLength(256)]
        public string Amount { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(RecipeId))]
        [InverseProperty("RecipeTools")]
        public virtual Recipe Recipe { get; set; }
        [ForeignKey(nameof(ToolId))]
        [InverseProperty("RecipeTools")]
        public virtual Tool Tool { get; set; }
        
        public override bool Equals(object? obj) {
            return GetHashCode() == obj?.GetHashCode();
        }

        public override int GetHashCode() {
            return Convert.ToInt32(ToolId);
        }
    }
}
