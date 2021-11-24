using Microsoft.AspNetCore.Mvc;

namespace RecipeManagementBE.Request.Search {
    public class SearchAccountDTO {
        
        /// <summary>
        /// Fullname account for searching
        /// </summary>
        [FromQuery(Name = "full_name")]
        public string FullName { get; set; }
        
        /// <summary>
        /// Email account for searching
        /// </summary>
        [FromQuery(Name = "email")]
        public string Email { get; set; }
        
        /// <summary>
        /// Id of brand account working (exclude role admin)
        /// </summary>
        [FromQuery(Name = "brand_id")]
        public long BrandId { get; set; }
        
        /// <summary>
        /// Id of role account in system
        /// </summary>
        [FromQuery(Name = "role_id")]
        public long RoleId { get; set; }
    }
}