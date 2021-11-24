using System.Collections.Generic;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Request.Search;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Service {
    public interface IAdminService {
        
        List<AdminDTO> GetAllAdmin();

        AdminDTO GetAdminById(long id);

        AdminDTO AddNewAdmin(RegisterDTO dto);
        
        bool DeleteAdminById(long id);
        
        PageResponse<AdminDTO> GetPageAdmin(PageableModel<SearchAccountDTO> pageableModel);
    }
}