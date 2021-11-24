using AutoMapper;
using JHipsterNet.Core.Pagination;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Mapper {
    public class AdminMapper : Profile {
        public AdminMapper() {
            CreateMap<BrandDTO, Brand>()
                .ReverseMap();

            // Pagination
            CreateMap<IPage<Brand>, PageResponse<BrandDTO>>();
        }
    }
}