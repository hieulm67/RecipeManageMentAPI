using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementBE.Constant;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Request.Search;
using RecipeManagementBE.Response;
using RecipeManagementBE.Service;

namespace RecipeManagementBE.Controllers {
    [ApiController]
    [Route(ApiPathURL.CATEGORY_API_PATH)]
    [Authorize]
    public class CategoryController : ControllerBase {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService) {
            _categoryService = categoryService;
        }
        
        /// <summary>
        /// Get all category available by brand id and name
        /// </summary>
        /// <param name="name">Name of category for searching</param>
        /// <param name="brandId">Id of brand contains category</param>
        /// <returns>List category matched</returns>
        /// GET: api/brands/categories
        [HttpGet(ApiPathURL.CATEGORY_BY_BRAND_API_PATH)]
        [Produces("application/json", Type = typeof(ResultJson<List<CategoryDTO>>))]
        public ActionResult<ResultJson<List<CategoryDTO>>> GetAllCategory([FromQuery] string name, [FromQuery] long brandId = 0) {
            return Ok(new ResultJson<List<CategoryDTO>> {
                Data = _categoryService.GetAllCategory(brandId, name),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Get all category available include dish is showing by brand id and name
        /// </summary>
        /// <param name="name">Name of category for searching</param>
        /// <param name="dishIsShow">Status is show of dish (optional)</param>
        /// <param name="brandId">Id of brand contains category</param>
        /// <returns>List category include dish matched</returns>
        /// GET: api/brands/categories/include-dish
        [HttpGet(ApiPathURL.CATEGORY_BY_BRAND_API_PATH + "/include-dish")]
        [Produces("application/json", Type = typeof(ResultJson<List<CategoryDTO>>))]
        public ActionResult<ResultJson<List<CategoryDTO>>> GetAllCategoryIncludeDish([FromQuery] string name, [FromQuery] bool? dishIsShow, [FromQuery] long brandId = 0) {
            return Ok(new ResultJson<List<CategoryDTO>> {
                Data = _categoryService.GetAllCategoryIncludeDish(brandId, name, dishIsShow),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Get category available by category id
        /// </summary>
        /// <param name="id">Id category for search</param>
        /// <param name="dishIsShow">Status is show of dish (optional)</param>
        /// <returns>Category matched</returns>
        /// GET: api/brands/categories/{id}
        [HttpGet(ApiPathURL.CATEGORY_BY_BRAND_API_PATH + "/{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<CategoryDTO>))]
        public ActionResult<ResultJson<CategoryDTO>> GetCategoryById([FromRoute]long id, [FromQuery] bool? dishIsShow) {
            return Ok(new ResultJson<CategoryDTO> {
                Data = _categoryService.GetCategoryById(id, dishIsShow),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Add new category to brand
        /// </summary>
        /// <param name="dto">Category info for create (brand id is required)</param>
        /// <returns>Category added</returns>
        /// POST: api/categories
        [HttpPost]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        [Produces("application/json", Type = typeof(ResultJson<CategoryDTO>))]
        public ActionResult<ResultJson<CategoryDTO>> AddNewCategoryToBrand([FromBody] CreateCategoryDTO dto) {
            return Ok(new ResultJson<CategoryDTO> {
                Data = _categoryService.AddNewCategoryToBrand(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Update category by id
        /// </summary>
        /// <param name="dto">Category info for update</param>
        /// <returns>Category updated</returns>
        /// PUT: api/categories
        [HttpPut]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        [Produces("application/json", Type = typeof(ResultJson<CategoryDTO>))]
        public ActionResult<ResultJson<CategoryDTO>> UpdateCategoryById([FromBody] CategoryDTO dto) {
            return Ok(new ResultJson<CategoryDTO> {
                Data = _categoryService.UpdateCategoryById(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Delete category by id
        /// </summary>
        /// <param name="id">Id of category want to delete</param>
        /// <returns>Delete status</returns>
        /// DELETE: api/categories/{id}
        [HttpDelete("{id:long}")]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        [Produces("application/json", Type = typeof(ResultJson<bool>))]
        public ActionResult<ResultJson<bool>> DeleteCategoryById([FromRoute] long id) {
            return Ok(new ResultJson<bool> {
                Data = _categoryService.DeleteCategoryById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get list category per page
        /// </summary>
        /// <param name="pageableModel">Pageable setting</param>
        /// <param name="dto">Search model</param>
        /// <returns>Page content list category</returns>
        /// POST: api/categories/pagination-filter
        [HttpGet("pagination-filter")]
        [Produces("application/json", Type = typeof(ResultJson<PageResponse<CategoryDTO>>))]
        public ActionResult<ResultJson<PageResponse<CategoryDTO>>> GetPageCategory([FromQuery] PageableModel<SearchCategoryDTO> pageableModel,
            [FromQuery] SearchCategoryDTO dto) {
            pageableModel.SearchModel = dto;
            
            return Ok(new ResultJson<PageResponse<CategoryDTO>> {
                Data = _categoryService.GetPageCategory(pageableModel),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get list category include dish per page
        /// </summary>
        /// <param name="pageableModel">Pageable setting</param>
        /// <param name="dto">Search model</param>
        /// <returns>Page content list category</returns>
        /// POST: api/categories/pagination-filter/include-dish
        [HttpGet("pagination-filter/include-dish")]
        [Produces("application/json", Type = typeof(ResultJson<PageResponse<CategoryDTO>>))]
        public ActionResult<ResultJson<PageResponse<CategoryDTO>>> GetPageCategoryIncludeDish([FromBody] PageableModel<SearchCategoryDTO> pageableModel,
            [FromQuery] SearchCategoryDTO dto) {
            pageableModel.SearchModel = dto;
            
            return Ok(new ResultJson<PageResponse<CategoryDTO>> {
                Data = _categoryService.GetPageCategoryIncludeDish(pageableModel),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
    }
}