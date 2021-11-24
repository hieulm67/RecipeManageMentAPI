using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeManagementBE.Entity
{
    [Table("Role")]
    [Index(nameof(Code), Name = "UQ__Role__A25C5AA7C878A431", IsUnique = true)]
    public class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
        }

        [Key]
        public long Id { get; set; }
        [StringLength(64)]
        public string Code { get; set; }
        [StringLength(128)]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        [InverseProperty(nameof(Account.Role))]
        public virtual ICollection<Account> Accounts { get; set; }
        
        public override string ToString() {
            return Name;
        }
    }
}
