using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity
{
    [Table("Log")]
    public class Log
    {
        [Key]
        public long Id { get; set; }
        public string Content { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LogTime { get; set; }
        [StringLength(128)]
        public string Type { get; set; }
    }
}
