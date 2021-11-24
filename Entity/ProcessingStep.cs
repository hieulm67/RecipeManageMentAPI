using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("ProcessingStep")]
    public class ProcessingStep
    {
        [Key]
        public long Id { get; set; }
        public long RecipeId { get; set; }
        public int StepNumber { get; set; }
        public string StepTitle { get; set; }
        public string StepContent { get; set; }
        public string ImageDescription { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(RecipeId))]
        [InverseProperty("ProcessingSteps")]
        public virtual Recipe Recipe { get; set; }
        
        public override string ToString() {
            return "Step Number " + StepNumber + ": " + StepTitle + "in Recipe Id: " + Recipe.Id + " of Dish " + Recipe.Dish.Name;
        }
        
        public override bool Equals(object? obj) {
            return GetHashCode() == obj?.GetHashCode();
        }

        public override int GetHashCode() {
            return Convert.ToInt32(StepNumber);
        }
    }
}
