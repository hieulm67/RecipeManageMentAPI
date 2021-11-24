using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeManagementBE.DTO {
    public class LogDTO {
        public long Id { get; set; }

        public string Content { get; set; }

        public DateTime? LogTime { get; set; }

        [StringLength(128)] public string Type { get; set; }
    }
}