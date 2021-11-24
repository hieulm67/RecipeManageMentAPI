using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace RecipeManagementBE.Entity
{
    [Table("Account")]
    public class Account
    {
        public Account()
        {
            Notifications = new HashSet<Notification>();
            Qas = new HashSet<Qa>();
        }

        [Key]
        [StringLength(64)]
        public string UID { get; set; }
        
        public long? RoleId { get; set; }
        
        [Required]
        [StringLength(64, MinimumLength = 6)]
        public string Password { get; set; }
        
        [StringLength(256)]
        public string Email { get; set; }
        
        [StringLength(128)]
        public string FullName { get; set; }
        
        [StringLength(15)]
        public string Phone { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(RoleId))]
        [InverseProperty("Accounts")]
        public virtual Role Role { get; set; }
        
        public virtual Admin Admin { get; set; }
        
        public virtual Employee Employee { get; set; }
        
        [InverseProperty(nameof(Notification.ToAccount))]
        public virtual ICollection<Notification> Notifications { get; set; }
        
        public virtual ICollection<Qa> Qas { get; set; }

        public override string ToString() {
            return "\"{currentAccount.FullName}\" with UID: {currentAccount.UID} and Role: {currentAccount.Role.Name} has been updated";
        }
    }
}
