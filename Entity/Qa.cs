using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("QA")]
    public class Qa
    {
        public Qa()
        {
        }

        [Key]
        public long Id { get; set; }
        [Column("QAId")]
        public long? QaParentId { get; set; }
        [StringLength(64)]
        public string UID { get; set; }
        public long RecipeId { get; set; }
        [Column("QATime", TypeName = "datetime")]
        public DateTime QaTime { get; set; } = DateTime.Now;
        public string Content { get; set; }
        public bool IsReply { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(UID))]
        public virtual Account Account { get; set; }
        
        [ForeignKey(nameof(QaParentId))]
        public virtual Qa QaParent { get; set; }
        
        public virtual Qa QaChild { get; set; }
        
        [ForeignKey(nameof(RecipeId))]
        [InverseProperty("Qas")]
        public virtual Recipe Recipe { get; set; }
        
        public override string ToString() {
            return "QA Id: " + Id;
        }
    }
}
