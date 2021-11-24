using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("Category")]
    public class Category
    {
        public Category()
        {
            Dishes = new HashSet<Dish>();
        }

        [Key]
        public long Id { get; set; }
        public long BrandId { get; set; }
        [StringLength(128)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(BrandId))]
        [InverseProperty("Categories")]
        public virtual Brand Brand { get; set; }
        [InverseProperty(nameof(Dish.Category))]
        public virtual ICollection<Dish> Dishes { get; set; }
        
        public override string ToString() {
            return Name + " of " + Brand.Name;
        }
    }
}
