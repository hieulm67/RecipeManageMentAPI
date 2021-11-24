using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RecipeManagementBE.Request {
    
    public class PageableModel<T> {
        
        /// <summary>
        /// Search model
        /// </summary>
        [FromQuery(Name = "search_model")]
        public T SearchModel { get; set; }

        /// <summary>
        /// Page number need to get
        /// </summary>
        [FromQuery(Name = "page_number")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Size per 1 page need to get
        /// </summary>
        [FromQuery(Name = "page_size")]
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Field for sorting
        /// </summary>
        [Range(-1, 1)]
        [FromQuery(Name = "sort_direction")]
        public int SortDirection { get; set; }
        
        /// <summary>
        /// Sort direction base on sort field
        /// </summary>
        [FromQuery(Name = "sort_field")]
        public string SortField { get; set; }
    }
}