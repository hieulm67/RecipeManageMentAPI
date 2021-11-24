using System;
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
    [Route(ApiPathURL.QA_API_PATH)]
    [Authorize]
    public class QAController : ControllerBase {

        private readonly IQAService _qaService;

        public QAController(IQAService qaService) {
            _qaService = qaService;
        }
        
        /// <summary>
        /// Get all qa available in recipe has id: recipeId
        /// </summary>
        /// <remarks>
        /// qa_time ex: 06/12/2021
        /// </remarks>
        /// <returns>List of qa</returns>
        /// GET: api/dishes/recipes/qas/
        [HttpGet(ApiPathURL.QA_BY_RECIPE_API_PATH)]
        [Produces("application/json", Type = typeof(ResultJson<List<QaDTO>>))]
        public ActionResult<ResultJson<List<QaDTO>>> GetAllRecipeByDishId([FromQuery] SearchQADTO dto) {
            return Ok(new ResultJson<List<QaDTO>> {
                Data = _qaService.GetAllQAByRecipeId(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get all qa available in recipe has id: recipeId (shortcut from notification)
        /// </summary>
        /// <remarks>
        /// qa_time ex: 06/12/2021
        /// </remarks>
        /// <returns>List of qa include marked qa from notification</returns>
        /// GET: api/dishes/recipes/qas/
        [HttpGet(ApiPathURL.QA_BY_RECIPE_API_PATH + "/notify")]
        [Produces("application/json", Type = typeof(ResultJson<ResultSearchQAIncludeMarkedId>))]
        [Authorize(Policy = RoleConstants.EMPLOYEE_CODE)]
        public ActionResult<ResultJson<ResultSearchQAIncludeMarkedId>> GetAllQAByRecipeIdIncludeMarkedQA([FromQuery] SearchQANotifiedDTO dto) {
            return Ok(new ResultJson<ResultSearchQAIncludeMarkedId> {
                Data = _qaService.GetAllQAByRecipeIdIncludeMarkedQA(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get qa available by id
        /// </summary>
        /// <param name="id">Id of qa need to get</param>
        /// <returns>List of qa include marked qa from notification</returns>
        /// GET: api/dishes/recipes/qas/
        [HttpGet(ApiPathURL.QA_BY_RECIPE_API_PATH + "{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<QaDTO>))]
        [Authorize(Policy = RoleConstants.EMPLOYEE_CODE)]
        public ActionResult<ResultJson<QaDTO>> GetQAById([FromRoute] long id) {
            return Ok(new ResultJson<QaDTO> {
                Data = _qaService.GetQAById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get list qa by recipeId
        /// </summary>
        /// <param name="pageableModel">Pageable setting</param>
        /// <param name="dto">Search model</param>
        /// <remarks>
        /// qa_time ex: 06/12/2021
        /// </remarks>
        /// <returns>Page content list qa</returns>
        /// POST: api/dishes/recipes/qas/pagination-filter
        [HttpGet(ApiPathURL.QA_BY_RECIPE_API_PATH + "/pagination-filter")]
        [Produces("application/json", Type = typeof(ResultJson<PageResponse<QaDTO>>))]
        public ActionResult<ResultJson<PageResponse<QaDTO>>> GetPageQAByRecipeId([FromQuery] PageableModel<SearchQADTO> pageableModel,
            [FromQuery] SearchQADTO dto) {
            pageableModel.SearchModel = dto;
            
            return Ok(new ResultJson<PageResponse<QaDTO>> {
                Data = _qaService.GetPageQAByRecipeId(pageableModel),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Add new qa
        /// </summary>
        /// <param name="dto">QA info for create</param>
        /// <returns>QA added</returns>
        /// POST: api/qas
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultJson<QaDTO>))]
        [Authorize(Policy = RoleConstants.EMPLOYEE_CODE)]
        public ActionResult<ResultJson<QaDTO>> PostNewQAInRecipe([FromBody] CreateQADTO dto) {
            return Ok(new ResultJson<QaDTO> {
                Data = _qaService.PostNewQAInRecipe(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Update QA by id
        /// </summary>
        /// <param name="dto">QA info for update</param>
        /// <returns>QA updated</returns>
        /// PUT: api/qas
        [HttpPut]
        [Produces("application/json", Type = typeof(ResultJson<QaDTO>))]
        [Authorize(Policy = RoleConstants.EMPLOYEE_CODE)]
        public ActionResult<ResultJson<QaDTO>> UpdateQAById([FromBody] QaDTO dto) {
            return Ok(new ResultJson<QaDTO> {
                Data = _qaService.UpdateQAById(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Delete qa by id
        /// </summary>
        /// <param name="dto">Id of qa want to delete</param>
        /// <returns>Delete status</returns>
        /// DELETE: api/qas/{id}
        [HttpDelete("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<bool>))]
        [Authorize(Policy = RoleConstants.EMPLOYEE_CODE)]
        public ActionResult<ResultJson<bool>> DeleteQAById([FromRoute] long id) {
            return Ok(new ResultJson<bool> {
                Data = _qaService.DeleteQAById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
    }
}