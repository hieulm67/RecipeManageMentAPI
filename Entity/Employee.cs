using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("Employee")]
    public class Employee
    {
        public Employee()
        {
            Dishes = new HashSet<Dish>();
        }

        [Key]
        public long Id { get; set; }
        [StringLength(64)]
        public string UID { get; set; }
        public long? BrandId { get; set; }
        public bool IsManager { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Account Account { get; set; }
        
        [ForeignKey(nameof(BrandId))]
        [InverseProperty("Employees")]
        public virtual Brand Brand { get; set; }
        
        [InverseProperty(nameof(Dish.Manager))]
        public virtual ICollection<Dish> Dishes { get; set; }

        public override string ToString() {
            return $"Account \"{Account.FullName}\" with UID: \"{UID}\" and Role: \"{Account.Role.Name}\" " +
                   $"of Brand: \"{Brand.Name}\"";
        }
    }
}
