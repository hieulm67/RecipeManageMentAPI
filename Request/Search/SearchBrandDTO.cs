using Microsoft.AspNetCore.Mvc;

namespace RecipeManagementBE.Request.Search {
    public class SearchBrandDTO {
        
        /// <summary>
        /// Name of brand for searching
        /// </summary>
        [FromQuery(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Address of brand for searching
        /// </summary>
        [FromQuery(Name = "address")]
        public string Address { get; set; }
    }
}