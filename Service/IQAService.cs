using System.Collections.Generic;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Request.Search;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Service {
    public interface IQAService {

        List<QaDTO> GetAllQAByRecipeId(SearchQADTO dto);
        
        ResultSearchQAIncludeMarkedId GetAllQAByRecipeIdIncludeMarkedQA(SearchQANotifiedDTO dto);
        
        PageResponse<QaDTO> GetPageQAByRecipeId(PageableModel<SearchQADTO> pageableModel);

        QaDTO GetQAById(long id);
        
        QaDTO PostNewQAInRecipe(CreateQADTO dto);
        
        QaDTO UpdateQAById(QaDTO dto);
        
        bool DeleteQAById(long id);

        bool DeleteQAsByAccountUID(string accountUID);
    }
}