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
    [Route(ApiPathURL.DISH_API_PATH)]
    [Authorize]
    public class DishController : ControllerBase {

        private readonly IDishService _dishService;

        public DishController(IDishService dishService) {
            _dishService = dishService;
        }
        
        /// <summary>
        /// Get all dish available by brand id and name and category ids
        /// </summary>
        /// <returns>List dish matched</returns>
        /// GET: api/brands/categories/dishes
        [HttpGet(ApiPathURL.DISH_BY_BRAND_API_PATH)]
        [Produces("application/json", Type = typeof(ResultJson<List<DishDTO>>))]
        public ActionResult<ResultJson<List<DishDTO>>> GetAllDish([FromQuery] SearchDishDTO dto) {
            return Ok(new ResultJson<List<DishDTO>> {
                Data = _dishService.GetAllDish(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get dish available by dish id
        /// </summary>
        /// <param name="id">Id dish for search</param>
        /// <returns>Dish matched</returns>
        /// GET: api/brands/categories/dishes/{id}
        [HttpGet(ApiPathURL.DISH_BY_BRAND_API_PATH + "/{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<DishDTO>))]
        public ActionResult<ResultJson<DishDTO>> GetDishById([FromRoute] long id) {
            return Ok(new ResultJson<DishDTO> {
                Data = _dishService.GetDishById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Add new dish to category
        /// </summary>
        /// <param name="dto">Dish info for create (category id is required)</param>
        /// <returns>Dish added</returns>
        /// POST: api/dishes
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultJson<DishDTO>))]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        public ActionResult<ResultJson<DishDTO>> AddNewDish([FromBody] CreateDishDTO dto) {
            return Ok(new ResultJson<DishDTO> {
                Data = _dishService.AddNewDish(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Update dish by id
        /// </summary>
        /// <param name="dto">Dish info for update</param>
        /// <returns>Dish updated</returns>
        /// PUT: api/dishes
        [HttpPut]
        [Produces("application/json", Type = typeof(ResultJson<DishDTO>))]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        public ActionResult<ResultJson<DishDTO>> UpdateDishById([FromBody] DishDTO dto) {
            return Ok(new ResultJson<DishDTO> {
                Data = _dishService.UpdateDishById(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Delete dish by id
        /// </summary>
        /// <param name="id">Id of dish want to delete</param>
        /// <returns>Delete status</returns>
        /// DELETE: api/dishes/{id}
        [HttpDelete("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<bool>))]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        public ActionResult<ResultJson<bool>> DeleteDishById([FromRoute] long id) {
            return Ok(new ResultJson<bool> {
                Data = _dishService.DeleteDishById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get list dish per page
        /// </summary>
        /// <param name="pageableModel">Pageable setting</param>
        /// <returns>Page content list dish</returns>
        /// GET: api/dishes/pagination-filter
        [HttpGet(ApiPathURL.DISH_BY_BRAND_API_PATH + "/pagination-filter")]
        [Produces("application/json", Type = typeof(ResultJson<PageResponse<DishDTO>>))]
        public ActionResult<ResultJson<PageResponse<DishDTO>>> GetPageCategory([FromQuery] PageableModel<SearchDishDTO> pageableModel,
            [FromQuery] SearchDishDTO dto) {
            pageableModel.SearchModel = dto;
            
            return Ok(new ResultJson<PageResponse<DishDTO>> {
                Data = _dishService.GetPageDish(pageableModel),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
    }
}