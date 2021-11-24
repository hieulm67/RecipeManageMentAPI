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
    [Route(ApiPathURL.TOOL_API_PATH)]
    [Authorize(Policy = RoleConstants.ADMIN_MANAGER_CODE)]
    public class ToolController : ControllerBase {

        private readonly IToolService _toolService;

        public ToolController(IToolService toolService) {
            _toolService = toolService;
        }
        
        /// <summary>
        /// Get all tool available
        /// </summary>
        /// <returns>List of tool</returns>
        /// GET: api/tool
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultJson<List<ToolDTO>>))]
        public ActionResult<ResultJson<List<ToolDTO>>> GetAllTool([FromQuery] string name) {
            return Ok(new ResultJson<List<ToolDTO>> {
                Data = _toolService.GetAllTool(name),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get tool by id
        /// </summary>
        /// <param name="id">Id of tool for searching</param>
        /// <returns>Tool matched</returns>
        /// GET: api/tool/{id}
        [HttpGet("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<ToolDTO>))]
        public ActionResult<ResultJson<ToolDTO>> GetToolById([FromRoute] long id) {
            return Ok(new ResultJson<ToolDTO> {
                Data = _toolService.GetToolById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Add new tool
        /// </summary>
        /// <param name="dto">Tool info for create</param>
        /// <returns>Tool added</returns>
        /// POST: api/tool
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultJson<ToolDTO>))]
        [Authorize(Policy = RoleConstants.MANAGER_CODE)]
        public ActionResult<ResultJson<ToolDTO>> AddNewTool([FromBody] ToolDTO dto) {
            return Ok(new ResultJson<ToolDTO> {
                Data = _toolService.AddNewTool(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Remove tool
        /// </summary>
        /// <param name="id">Id of tool want to delete</param>
        /// <returns>Delete status</returns>
        /// DELETE: api/tool
        [HttpDelete("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<bool>))]
        [Authorize(Policy = RoleConstants.ADMIN_CODE)]
        public ActionResult<ResultJson<bool>> DeleteToolById([FromRoute] long id) {
            return Ok(new ResultJson<bool> {
                Data = _toolService.DeleteToolById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Get list tool per page
        /// </summary>
        /// <remarks>Search model is toolName</remarks>
        /// <returns>Page content list tool</returns>
        /// GET: api/tool/pagination-filter
        [HttpGet("pagination-filter")]
        [Produces("application/json", Type = typeof(ResultJson<PageResponse<ToolDTO>>))]
        public ActionResult<ResultJson<PageResponse<ToolDTO>>> GetPageTool([FromQuery] PageableModel<string> pageableModel) {
            return Ok(new ResultJson<PageResponse<ToolDTO>> {
                Data = _toolService.GetPageTool(pageableModel),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
    }
}