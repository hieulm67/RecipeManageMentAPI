using System;

#nullable disable

namespace RecipeManagementBE.DTO {
    public class QaDTO {
        public long Id { get; set; }

        public DateTime QaTime { get; set; }
        
        public long QaParentId { get; set; }

        public string Content { get; set; }

        public bool IsReply { get; set; }

        public AccountDTO Account { get; set; }

        public long RecipeId { get; set; }
        
        public long DishId { get; set; }

        public QaDTO QaChild { get; set; }
    }
}