using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("Tool")]
    public class Tool
    {
        public Tool()
        {
            RecipeTools = new HashSet<RecipeTool>();
        }

        [Key]
        public long Id { get; set; }
        
        [StringLength(128)]
        public string Name { get; set; }
        
        public bool IsDeleted { get; set; }

        [InverseProperty(nameof(RecipeTool.Tool))]
        public virtual ICollection<RecipeTool> RecipeTools { get; set; }
        
        public override string ToString() {
            return Name;
        }
    }
}
