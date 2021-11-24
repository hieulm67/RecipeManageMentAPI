using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("Brand")]
    public class Brand
    {
        public Brand()
        {
            Categories = new HashSet<Category>();
            Employees = new HashSet<Employee>();
        }

        [Key]
        public long Id { get; set; }
        [StringLength(128)]
        public string Name { get; set; }
        public string Logo { get; set; }
        [StringLength(15)]
        public string Phone { get; set; }
        public string Address { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        public string HomePage { get; set; }
        public bool IsDeleted { get; set; }

        [InverseProperty(nameof(Category.Brand))]
        public virtual ICollection<Category> Categories { get; set; }
        [InverseProperty(nameof(Employee.Brand))]
        public virtual ICollection<Employee> Employees { get; set; }

        public override string ToString() {
            return Name;
        }
    }
}
