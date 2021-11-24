using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementBE.Constant;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request;
using RecipeManagementBE.Response;
using RecipeManagementBE.Service;

namespace RecipeManagementBE.Controllers {
    
    [ApiController]
    [Route(ApiPathURL.INGREDIENT_API_PATH)]
    [Authorize(Policy = RoleConstants.ADMIN_MANAGER_CODE)]
    public class IngredientController : ControllerBase {
        
        private readonly IIngredientService _ingredientService;

        public IngredientController(IIngredientService ingredientService) {
            _ingredientService = ingredientService;
        }
        
        /// <summary>
        /// Get all ingredient available
        /// </summary>
        /// <param name="name">Name of ingredient for searching</param>
        /// <returns>List of ingredient</returns>
        /// GET: api/ingredients
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultJson<List<IngredientDTO>>))]
        public ActionResult<ResultJson<List<IngredientDTO>>> GetAllIngredient([FromQuery] string name) {
            return Ok(new ResultJson<List<IngredientDTO>> {
                Data = _ingredientService.GetAllIngredient(name),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get ingredient by id
        /// </summary>
        /// <param name="id">Id of ingredient for searching</param>
        /// <returns>Ingredient matched</returns>
        /// GET: api/ingredients/{id}
        [HttpGet("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<IngredientDTO>))]
        public ActionResult<ResultJson<IngredientDTO>> GetIngredientById([FromRoute] long id) {
            return Ok(new ResultJson<IngredientDTO> {
                Data = _ingredientService.GetIngredientById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Add new ingredient
        /// </summary>
        /// <param name="dto">Ingredient info for create</param>
        /// <returns>Ingredient added</returns>
        /// POST: api/ingredients
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultJson<IngredientDTO>))]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        public ActionResult<ResultJson<IngredientDTO>> AddNewIngredient([FromBody] IngredientDTO dto) {
            return Ok(new ResultJson<IngredientDTO> {
                Data = _ingredientService.AddNewIngredient(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Remove ingredient
        /// </summary>
        /// <param name="id">Id of ingredient want to delete</param>
        /// <returns>Delete status</returns>
        /// DELETE: api/ingredients
        [HttpDelete("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<bool>))]
        [Authorize(Policy = RoleConstants.ADMIN_CODE)]
        public ActionResult<ResultJson<bool>> DeleteIngredientById([FromRoute] long id) {
            return Ok(new ResultJson<bool> {
                Data = _ingredientService.DeleteIngredientById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Get list ingredient per page
        /// </summary>
        /// <remarks>Search model is ingredientName</remarks>
        /// <returns>Page content list ingredient</returns>
        /// POST: api/ingredients/pagination-filter
        [HttpGet("pagination-filter")]
        [Produces("application/json", Type = typeof(ResultJson<PageResponse<IngredientDTO>>))]
        public ActionResult<ResultJson<PageResponse<IngredientDTO>>> GetPageIngredient([FromQuery] PageableModel<string> pageableModel) {
            return Ok(new ResultJson<PageResponse<IngredientDTO>> {
                Data = _ingredientService.GetPageIngredient(pageableModel),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
    }
}