using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementBE.Constant;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Request.Search;
using RecipeManagementBE.Response;
using RecipeManagementBE.Service;

namespace RecipeManagementBE.Controllers {
    [ApiController]
    [Route(ApiPathURL.BRAND_API_PATH)]
    [Authorize(Policy = RoleConstants.ADMIN_CODE)]
    public class BrandController : ControllerBase {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService) {
            _brandService = brandService;
        }

        /// <summary>
        /// Get all brand available
        /// </summary>
        /// <returns>List of brand</returns>
        /// GET: api/brands
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultJson<List<BrandDTO>>))]
        public ActionResult<ResultJson<List<BrandDTO>>> GetAllBrand() {
            return Ok(new ResultJson<List<BrandDTO>> {
                Data = _brandService.GetAllBrand(),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Get brand by id
        /// </summary>
        /// <param name="id">Id of brand for searching</param>
        /// <returns>brand matched</returns>
        /// GET: api/brands/{id}
        [HttpGet("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<BrandDTO>))]
        public ActionResult<ResultJson<BrandDTO>> GetBrandById([FromRoute] long id) {
            return Ok(new ResultJson<BrandDTO> {
                Data = _brandService.GetBrandById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Add new brand
        /// </summary>
        /// <param name="dto">Brand info for create</param>
        /// <returns>Brand added</returns>
        /// POST: api/brands
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultJson<BrandDTO>))]
        public ActionResult<ResultJson<BrandDTO>> AddNewBrand([FromBody] BrandDTO dto) {
            return Ok(new ResultJson<BrandDTO> {
                Data = _brandService.AddNewBrand(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Update brand info
        /// </summary>
        /// <param name="dto">Brand info for update</param>
        /// <returns>Brand updated</returns>
        /// PUT: api/brands
        [HttpPut]
        [Produces("application/json", Type = typeof(ResultJson<BrandDTO>))]
        public ActionResult<ResultJson<BrandDTO>> UpdateBrandById([FromBody] BrandDTO dto) {
            return Ok(new ResultJson<BrandDTO> {
                Data = _brandService.UpdateBrandById(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Remove brand
        /// </summary>
        /// <param name="id">Id of brand want to delete</param>
        /// <returns>Delete status</returns>
        /// DELETE: api/brands
        [HttpDelete("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<bool>))]
        public ActionResult<ResultJson<bool>> DeleteBrandById([FromRoute] long id) {
            return Ok(new ResultJson<bool> {
                Data = _brandService.DeleteBrandById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Get list brand per page
        /// </summary>
        /// <param name="pageableModel">Pageable setting</param>
        /// <param name="dto">Search model</param>
        /// <returns>Page content list brand</returns>
        /// GET: api/brands/pagination-filter
        [HttpGet("pagination-filter")]
        [Produces("application/json", Type = typeof(ResultJson<PageResponse<BrandDTO>>))]
        public ActionResult<ResultJson<PageResponse<BrandDTO>>> GetPageBrand([FromQuery] PageableModel<SearchBrandDTO> pageableModel,
            [FromQuery] SearchBrandDTO dto) {
            pageableModel.SearchModel = dto;
            
            return Ok(new ResultJson<PageResponse<BrandDTO>> {
                Data = _brandService.GetPageBrand(pageableModel),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
    }
}