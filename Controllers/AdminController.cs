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
    [Route(ApiPathURL.ADMIN_API_PATH)]
    [ApiController]
    [Authorize(Policy = RoleConstants.ADMIN_CODE)]
    public class AdminController : ControllerBase {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService) {
            _adminService = adminService;
        }

        /// <summary>
        /// Get all admin available
        /// </summary>
        /// <returns>List of admin</returns>
        /// GET: api/admin
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultJson<List<AdminDTO>>))]
        public ActionResult<ResultJson<List<AdminDTO>>> GetAllAdmin() {
            return Ok(new ResultJson<List<AdminDTO>> {
                Data = _adminService.GetAllAdmin(),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Get admin by id
        /// </summary>
        /// <param name="id">Id of admin for searching</param>
        /// <returns>Admin matched</returns>
        /// GET: api/admin/{id}
        [HttpGet("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<AdminDTO>))]
        public ActionResult<ResultJson<AdminDTO>> GetAdminById([FromRoute] long id) {
            return Ok(new ResultJson<AdminDTO> {
                Data = _adminService.GetAdminById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Add new admin
        /// </summary>
        /// <param name="dto">Admin info for create</param>
        /// <returns>Admin added</returns>
        /// POST: api/admin
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultJson<AdminDTO>))]
        public ActionResult<ResultJson<AdminDTO>> AddNewAdmin([FromBody] RegisterDTO dto) {
            return Ok(new ResultJson<AdminDTO> {
                Data = _adminService.AddNewAdmin(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Remove admin
        /// </summary>
        /// <param name="id">Id of admin want to delete</param>
        /// <returns>Delete status</returns>
        /// DELETE: api/admin
        [HttpDelete("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<bool>))]
        [Authorize(Policy = RoleConstants.ADMIN_SYSTEM)]
        public ActionResult<ResultJson<bool>> DeleteAdminById([FromRoute] long id) {
            return Ok(new ResultJson<bool> {
                Data = _adminService.DeleteAdminById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Get list admin per page
        /// </summary>
        /// <param name="pageableModel">Pageable setting</param>
        /// <param name="dto">Search model</param>
        /// <returns>Page content list admin</returns>
        /// GET: api/admin/pagination-filter
        [HttpGet("pagination-filter")]
        [Produces("application/json", Type = typeof(ResultJson<PageResponse<AdminDTO>>))]
        public ActionResult<ResultJson<PageResponse<AdminDTO>>> GetPageAdmin([FromQuery] PageableModel<SearchAccountDTO> pageableModel, 
            [FromQuery] SearchAccountDTO dto) {
            pageableModel.SearchModel = dto;
            
            return Ok(new ResultJson<PageResponse<AdminDTO>> {
                Data = _adminService.GetPageAdmin(pageableModel),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
    }
}