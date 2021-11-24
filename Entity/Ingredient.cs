using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("Ingredient")]
    public class Ingredient
    {
        public Ingredient()
        {
            RecipeDetails = new HashSet<RecipeDetail>();
        }

        [Key]
        public long Id { get; set; }
        
        [StringLength(128)]
        public string Name { get; set; }
        
        public bool IsDeleted { get; set; }

        [InverseProperty(nameof(RecipeDetail.Ingredient))]
        public virtual ICollection<RecipeDetail> RecipeDetails { get; set; }
        
        public override string ToString() {
            return Name;
        }
    }
}
