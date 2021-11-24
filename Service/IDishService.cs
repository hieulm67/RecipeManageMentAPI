using System.Collections.Generic;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Request.Search;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Service {
    public interface IDishService {

        List<DishDTO> GetAllDish(SearchDishDTO dto);
        
        DishDTO GetDishById(long id);
        
        PageResponse<DishDTO> GetPageDish(PageableModel<SearchDishDTO> pageableModel);

        DishDTO AddNewDish(CreateDishDTO dto);

        DishDTO UpdateDishById(DishDTO dto);

        bool DeleteDishById(long id);

        bool DeleteDishesByCategoryId(long categoryId);

        bool UpdateDishesCreateByManagerId(long managerId);
    }
}