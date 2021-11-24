using System;

namespace RecipeManagementBE.DTO {
    public class NotificationDTO {
        public long Id { get; set; }

        public DateTime SendingTime { get; set; }

        public string PayloadContent { get; set; }

        public string To { get; set; }

        public bool? IsSent { get; set; }

        public bool? IsSeen { get; set; }
        
        public string Type { get; set; }
        
        public long NotifiedId { get; set; }
    }
}