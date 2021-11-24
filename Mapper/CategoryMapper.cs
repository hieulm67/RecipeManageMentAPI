using AutoMapper;
using JHipsterNet.Core.Pagination;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Mapper {
    public class CategoryMapper : Profile {
        public CategoryMapper() {
            CreateMap<CategoryDTO, Category>()
                .ForMember(category => category.BrandId, opt => opt.Ignore())
                .ForMember(category => category.Dishes, opt => opt.Ignore());

            CreateMap<Category, CategoryDTO>();
            // Pagination
            CreateMap<IPage<Category>, PageResponse<CategoryDTO>>();
            
            CreateMap<CreateCategoryDTO, Category>();
        }
    }
}