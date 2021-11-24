using System.Collections.Generic;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Service {
    public interface IToolService {
        
        List<ToolDTO> GetAllTool(string name);

        ToolDTO GetToolById(long id);

        ToolDTO AddNewTool(ToolDTO dto);
        
        bool DeleteToolById(long id);
        
        PageResponse<ToolDTO> GetPageTool(PageableModel<string> pageableModel);
    }
}