using System.Collections.Generic;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Request.Search;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Service {
    public interface ICategoryService {

        List<CategoryDTO> GetAllCategory(long brandId, string name);

        List<CategoryDTO> GetAllCategoryIncludeDish(long brandId, string name, bool? dishIsShow);

        CategoryDTO GetCategoryById(long categoryId, bool? dishIsShow);

        CategoryDTO AddNewCategoryToBrand(CreateCategoryDTO dto);
        
        CategoryDTO UpdateCategoryById(CategoryDTO dto);

        bool DeleteCategoryById(long id);

        bool DeleteCategoriesByBrandId(long brandId);
        
        PageResponse<CategoryDTO> GetPageCategory(PageableModel<SearchCategoryDTO> pageableModel);
        
        PageResponse<CategoryDTO> GetPageCategoryIncludeDish(PageableModel<SearchCategoryDTO> pageableModel);
    }
}