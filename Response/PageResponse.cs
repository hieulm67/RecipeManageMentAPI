using System.Collections.Generic;

namespace RecipeManagementBE.Response {
    public class PageResponse<T> {
        
        public int TotalPages { get; set; }
        
        public int TotalElements { get; set; }
        
        public bool HasNext { get; set; }
        
        public bool HasPrevious { get; set; }

        public bool IsLast { get; set; }
        
        public bool IsFirst { get; set; }
        
        public int Number { get; set; }
        
        public int Size { get; set; }
        
        public List<T> Content { get; set; }
    }
}