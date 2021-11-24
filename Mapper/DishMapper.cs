using AutoMapper;
using JHipsterNet.Core.Pagination;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Mapper {
    public class DishMapper : Profile {
        public DishMapper() {
            CreateMap<DishDTO, Dish>()
                .ForMember(dish => dish.Manager, opt => opt.Ignore())
                .ForMember(dish => dish.ManagerId, opt => opt.Ignore())
                .ForMember(dish => dish.Category, opt => opt.Ignore())
                .ForMember(dish => dish.CategoryId, opt => opt.Ignore())
                .ForMember(dish => dish.Recipes, opt => opt.Ignore());
            
            CreateMap<Dish, DishDTO>()
                .ForMember(dto => dto.CategoryName, opt => opt.MapFrom(dish => dish.Category.Name));
            
            CreateMap<CreateDishDTO, Dish>();
            
            // Pagination
            CreateMap<IPage<Dish>, PageResponse<DishDTO>>();
        }
    }
}