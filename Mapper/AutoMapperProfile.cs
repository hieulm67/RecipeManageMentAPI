using AutoMapper;
using JHipsterNet.Core.Pagination;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Mapper {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            
            // DTO to Entity map 
            CreateMap<IngredientDTO, Ingredient>()
                .ReverseMap();
            CreateMap<ToolDTO, Tool>()
                .ReverseMap();
            CreateMap<NotificationDTO, Notification>()
                .ReverseMap();
            CreateMap<LogDTO, Log>()
                .ReverseMap();
            
            
            // Pagination
            CreateMap<IPage<Tool>, PageResponse<ToolDTO>>();
            CreateMap<IPage<Ingredient>, PageResponse<IngredientDTO>>();
            CreateMap<IPage<Notification>, PageResponse<NotificationDTO>>();
        }
    }
}