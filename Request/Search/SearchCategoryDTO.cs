using Microsoft.AspNetCore.Mvc;

namespace RecipeManagementBE.Request.Search {
    public class SearchCategoryDTO {
        
        /// <summary>
        /// Id of brand contains category
        /// </summary>
        [FromQuery(Name = "brand_id")]
        public long BrandId { get; set; }

        /// <summary>
        /// Name of category for searching
        /// </summary>
        [FromQuery(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Status is show of dish (optional)
        /// </summary>
        [FromQuery(Name = "dish_is_show")]
        public bool? DishIsShow { get; set; }
    }
}