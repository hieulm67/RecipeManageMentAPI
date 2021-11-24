using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace RecipeManagementBE.Request.Search {
    public class SearchDishDTO {
        
        /// <summary>
        /// Id of brand contains dish
        /// </summary>
        [FromQuery(Name = "brand_id")]
        public long BrandId { get; set; }

        /// <summary>
        /// List ids of category contains dish
        /// </summary>
        [FromQuery(Name = "categories_id")]
        public List<long> CategoriesId { get; set; }

        /// <summary>
        /// List ids of tool that dish is using 
        /// </summary>
        [FromQuery(Name = "tools_id")]
        public List<long> ToolsId { get; set; }
        
        /// <summary>
        /// List ids of ingredient that dish is using 
        /// </summary>
        [FromQuery(Name = "ingredients_id")]
        public List<long> IngredientsId { get; set; }

        /// <summary>
        /// Name of dish for searching
        /// </summary>
        [FromQuery(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Status is show of dish 
        /// </summary>
        [FromQuery(Name = "dish_is_show")]
        public bool? DishIsShow { get; set; }
    }
}