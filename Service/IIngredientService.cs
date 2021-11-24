using System.Collections.Generic;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Service {
    public interface IIngredientService {
        
        List<IngredientDTO> GetAllIngredient(string name);

        IngredientDTO GetIngredientById(long id);

        IngredientDTO AddNewIngredient(IngredientDTO dto);
        
        bool DeleteIngredientById(long id);
        
        PageResponse<IngredientDTO> GetPageIngredient(PageableModel<string> pageableModel);
    }
}