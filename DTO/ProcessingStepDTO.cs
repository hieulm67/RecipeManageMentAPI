using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeManagementBE.DTO
{
    public class ProcessingStepDTO
    {
        [Required]
        public int StepNumber { get; set; }
        
        public string StepTitle { get; set; }
        
        public string StepContent { get; set; }
        
        public string ImageDescription { get; set; }
        
        public override bool Equals(object? obj) {
            return GetHashCode() == obj?.GetHashCode();
        }

        public override int GetHashCode() {
            return Convert.ToInt32(StepNumber);
        }
    }
}
