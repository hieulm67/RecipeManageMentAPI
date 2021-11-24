using System;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Util {
    public static class PaginationUtil {

        public static PageResponse<T> GetTotalPage<T>(this PageResponse<T> pageResponse) {
            var totalElements = pageResponse.TotalElements;
            var pageSize = pageResponse.Size;

            var totalPage = Math.Ceiling((double) totalElements / pageSize);
            pageResponse.TotalPages = (int) totalPage;
            
            return pageResponse;
        }
    }
}