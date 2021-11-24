using System.Collections.Generic;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Request.Search;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Service {
    public interface IEmployeeService {
        
        List<EmployeeDTO> GetAllEmployee();

        EmployeeDTO GetEmployeeById(long id);

        EmployeeDTO AddNewEmployee(RegisterDTO dto);
        
        EmployeeDTO UpdateEmployeeRoleById(long id, bool isManager);

        bool DeleteEmployeeById(long id);

        bool DeleteEmployeesByBrandId(long brandId);

        PageResponse<EmployeeDTO> GetPageEmployee(PageableModel<SearchAccountDTO> pageableModel);
    }
}