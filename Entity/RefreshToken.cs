using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeManagementBE.Entity {
    
    [Table("RefreshToken")]
    public class RefreshToken {

        public Guid Id { get; set; }
        
        public string Token { get; set; }

        public string JwtId { get; set; }

        public string UserUID { get; set; }

        public bool IsUsed { get; set; }
        
        public bool IsRevoked { get; set; }

        public DateTime AddedDate { get; set; }
        
        public DateTime ExpiryDate { get; set; }
    }
}