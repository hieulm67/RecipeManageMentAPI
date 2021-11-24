using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("Dish")]
    public class Dish
    {
        public Dish()
        {
            Recipes = new HashSet<Recipe>();
        }

        [Key]
        public long Id { get; set; }
        public long? CategoryId { get; set; }
        public long? ManagerId { get; set; }
        [StringLength(128)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsShow { get; set; }
        
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(CategoryId))]
        [InverseProperty("Dishes")]
        public virtual Category Category { get; set; }
        [ForeignKey(nameof(ManagerId))]
        [InverseProperty(nameof(Employee.Dishes))]
        public virtual Employee Manager { get; set; }
        [InverseProperty(nameof(Recipe.Dish))]
        public virtual ICollection<Recipe> Recipes { get; set; }
        
        public override string ToString() {
            return Name + " in Category " + Category.Name + " of " + Category.Brand.Name;
        }
    }
}
