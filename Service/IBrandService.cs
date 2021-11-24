using System.Collections.Generic;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Request.Search;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Service {
    public interface IBrandService {
        
        List<BrandDTO> GetAllBrand();

        BrandDTO GetBrandById(long id);

        BrandDTO AddNewBrand(BrandDTO dto);
        
        BrandDTO UpdateBrandById(BrandDTO dto);

        bool DeleteBrandById(long id);
        
        PageResponse<BrandDTO> GetPageBrand(PageableModel<SearchBrandDTO> pageableModel);
    }
}