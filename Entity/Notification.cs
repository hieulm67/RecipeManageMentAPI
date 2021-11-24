using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("Notification")]
    public class Notification
    {
        [Key]
        public long Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime SendingTime { get; set; } = DateTime.Now;
        public string PayloadContent { get; set; }
        [StringLength(64)]
        public string To { get; set; }

        public bool IsSent { get; set; } = true;
        public bool IsSeen { get; set; }

        public string Type { get; set; }
        
        public long NotifiedId { get; set; }

        [ForeignKey(nameof(To))]
        [InverseProperty(nameof(Account.Notifications))]
        public virtual Account ToAccount { get; set; }
    }
}
