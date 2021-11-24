using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("RecipeDetail")]
    public class RecipeDetail
    {
        [Key]
        public long Id { get; set; }
        public long RecipeId { get; set; }
        public long IngredientId { get; set; }
        [StringLength(256)]
        public string Amount { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(IngredientId))]
        [InverseProperty("RecipeDetails")]
        public virtual Ingredient Ingredient { get; set; }
        [ForeignKey(nameof(RecipeId))]
        [InverseProperty("RecipeDetails")]
        public virtual Recipe Recipe { get; set; }
        
        public override bool Equals(object? obj) {
            return GetHashCode() == obj?.GetHashCode();
        }

        public override int GetHashCode() {
            return Convert.ToInt32(IngredientId);
        }
    }
}
