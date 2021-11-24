using System;
using Microsoft.AspNetCore.Mvc;

namespace RecipeManagementBE.Request.Search {
    public class SearchQADTO {
        
        /// <summary>
        /// Recipe id contains list qa
        /// </summary>
        [FromQuery(Name = "recipe_id")]
        public long RecipeId { get; set; }
        
        /// <summary>
        /// Qa posted time
        /// </summary>
        [FromQuery(Name = "qa_time")]
        public DateTime QATime { get; set; }
    }
}