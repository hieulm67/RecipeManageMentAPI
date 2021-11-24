using System.Collections.Generic;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Request.Search;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Service {
    public interface IRecipeService {
        
        List<RecipeDTO> GetAllRecipeByDishId(long dishId);

        RecipeDTO GetRecipeById(long id);

        PageResponse<RecipeDTO> GetPageRecipe(PageableModel<long> pageableModel);

        RecipeDTO AddNewRecipe(CreateRecipeDTO dto);

        RecipeDTO UpdateRecipeById(RecipeDTO dto);
        
        RecipeDTO UpdateRecipeIsUsingById(long id);

        bool DeleteRecipeById(long id);

        bool DeleteRecipesByDishId(long dishId);
    }
}