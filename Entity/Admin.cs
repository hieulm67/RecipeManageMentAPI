using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("Admin")]
    public class Admin
    {
        [Key]
        public long Id { get; set; }
        
        [StringLength(64)]
        public string UID { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Account Account { get; set; }

        public override string ToString() {
            return $"Account \"{Account.FullName}\" with UID: \"{UID}\" and Role: \"{Account.Role.Name}\"";
        }
    }
}
