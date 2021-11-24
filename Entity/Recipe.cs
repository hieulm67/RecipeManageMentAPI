using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("Recipe")]
    public class Recipe
    {
        public Recipe()
        {
            ProcessingSteps = new HashSet<ProcessingStep>();
            Qas = new HashSet<Qa>();
            RecipeDetails = new HashSet<RecipeDetail>();
            RecipeTools = new HashSet<RecipeTool>();
        }

        [Key]
        public long Id { get; set; }
        public long DishId { get; set; }
        public string Description { get; set; }
        public string ImageDescription { get; set; }
        public bool IsUsing { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(DishId))]
        [InverseProperty("Recipes")]
        public virtual Dish Dish { get; set; }
        [InverseProperty(nameof(ProcessingStep.Recipe))]
        public virtual ICollection<ProcessingStep> ProcessingSteps { get; set; }
        [InverseProperty(nameof(Qa.Recipe))]
        public virtual ICollection<Qa> Qas { get; set; }
        [InverseProperty(nameof(RecipeDetail.Recipe))]
        public virtual ICollection<RecipeDetail> RecipeDetails { get; set; }
        [InverseProperty(nameof(RecipeTool.Recipe))]
        public virtual ICollection<RecipeTool> RecipeTools { get; set; }
        
        public override string ToString() {
            return "Id: " + Id + " of Dish " + Dish.Name;
        }
    }
}
