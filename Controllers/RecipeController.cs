using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementBE.Constant;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Response;
using RecipeManagementBE.Service;

namespace RecipeManagementBE.Controllers {
    
    [ApiController]
    [Route(ApiPathURL.RECIPE_BY_DISH_API_PATH)]
    [Authorize]
    public class RecipeController : ControllerBase {

        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService) {
            _recipeService = recipeService;
        }
        
        /// <summary>
        /// Get all recipe available
        /// </summary>
        /// <param name="dishId">Id of dish contains recipes</param>
        /// <returns>List of recipe</returns>
        /// GET: api/recipes
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultJson<List<RecipeDTO>>))]
        public ActionResult<ResultJson<List<RecipeDTO>>> GetAllRecipeByDishId([FromQuery] long dishId) {
            return Ok(new ResultJson<List<RecipeDTO>> {
                Data = _recipeService.GetAllRecipeByDishId(dishId),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get recipe by id
        /// </summary>
        /// <param name="id">Id of recipe for searching</param>
        /// <returns>Recipe matched</returns>
        /// GET: api/recipes/{id}
        [HttpGet("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<RecipeDTO>))]
        public ActionResult<ResultJson<RecipeDTO>> GetRecipeById([FromRoute] long id) {
            return Ok(new ResultJson<RecipeDTO> {
                Data = _recipeService.GetRecipeById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get list recipe by dishId
        /// </summary>
        /// <remarks>Search model is dishId</remarks>
        /// <returns>Page content list recipe</returns>
        /// GET: api/recipes/pagination-filter
        [HttpGet("pagination-filter")]
        [Produces("application/json", Type = typeof(ResultJson<PageResponse<RecipeDTO>>))]
        public ActionResult<ResultJson<PageResponse<RecipeDTO>>> GetPageRecipeByDishId([FromQuery] PageableModel<long> pageableModel) {
            return Ok(new ResultJson<PageResponse<RecipeDTO>> {
                Data = _recipeService.GetPageRecipe(pageableModel),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Add new recipe
        /// </summary>
        /// <param name="dto">Recipe info for create</param>
        /// <returns>Recipe added</returns>
        /// POST: api/recipes
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultJson<RecipeDTO>))]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        public ActionResult<ResultJson<RecipeDTO>> AddNewRecipe([FromBody] CreateRecipeDTO dto) {
            return Ok(new ResultJson<RecipeDTO> {
                Data = _recipeService.AddNewRecipe(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Update recipe by id
        /// </summary>
        /// <param name="dto">Recipe info for update</param>
        /// <returns>Recipe updated</returns>
        /// PUT: api/recipes
        [HttpPut]
        [Produces("application/json", Type = typeof(ResultJson<RecipeDTO>))]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        public ActionResult<ResultJson<RecipeDTO>> UpdateRecipeById([FromBody] RecipeDTO dto) {
            return Ok(new ResultJson<RecipeDTO> {
                Data = _recipeService.UpdateRecipeById(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Update recipe by id to using
        /// </summary>
        /// <param name="id">Id of recipe update to is using</param>
        /// <returns>Recipe updated</returns>
        /// PUT: api/recipes
        [HttpPatch]
        [Produces("application/json", Type = typeof(ResultJson<RecipeDTO>))]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        public ActionResult<ResultJson<RecipeDTO>> UpdateRecipeIsUsingById([FromBody] long id) {
            return Ok(new ResultJson<RecipeDTO> {
                Data = _recipeService.UpdateRecipeIsUsingById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Delete recipe by id
        /// </summary>
        /// <param name="id">Id of recipe need to delete</param>
        /// <returns>Delete status</returns>
        /// DELETE: api/recipes
        [HttpDelete("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<bool>))]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        public ActionResult<ResultJson<bool>> DeleteRecipeById([FromRoute] long id) {
            return Ok(new ResultJson<bool> {
                Data = _recipeService.DeleteRecipeById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
    }
}