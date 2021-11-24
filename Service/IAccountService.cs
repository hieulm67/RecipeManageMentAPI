using System.Collections.Generic;
using RecipeManagementBE.DTO;

namespace RecipeManagementBE.Service {
    public interface IAccountService {

        List<RoleDTO> GetAllRole();

        object GetCurrentAccountInfo();

        AccountDTO UpdateAccountInfo(AccountDTO dto);

        AccountDTO UpdateAccountPasswordByUIDAndEmail(AccountDTO dto);

        bool CheckPasswordCurrentAccount(string password);
    }
}