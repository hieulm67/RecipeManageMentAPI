using Microsoft.AspNetCore.Mvc;

namespace RecipeManagementBE.Request.Search {
    public class SearchQANotifiedDTO {
        
        /// <summary>
        /// Recipe id contains list qa
        /// </summary>
        [FromQuery(Name = "recipe_id")]
        public long RecipeId { get; set; }

        /// <summary>
        /// Id of qa notified in notification
        /// </summary>
        [FromQuery(Name = "notified_id")]
        public long NotifiedId { get; set; }
        
        /// <summary>
        /// Account UID received notification
        /// </summary>
        [FromQuery(Name = "to")]
        public string To { get; set; }
    }
}