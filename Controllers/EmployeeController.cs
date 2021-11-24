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
    
    [Route(ApiPathURL.EMPLOYEE_API_PATH)]
    [ApiController]
    [Authorize(Policy = RoleConstants.ADMIN_MANAGER_CODE)]
    public class EmployeeController : ControllerBase {

        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService) {
            _employeeService = employeeService;
        }
        
        /// <summary>
        /// Get all employee available
        /// </summary>
        /// <returns>List of employee</returns>
        /// GET: api/employee
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultJson<List<EmployeeDTO>>))]
        public ActionResult<ResultJson<List<EmployeeDTO>>> GetAllEmployee() {
            return Ok(new ResultJson<List<EmployeeDTO>> {
                Data = _employeeService.GetAllEmployee(),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Get employee by id
        /// </summary>
        /// <param name="id">Id of employee for searching</param>
        /// <returns>Employee matched</returns>
        /// GET: api/employee/{id}
        [HttpGet("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<EmployeeDTO>))]
        public ActionResult<ResultJson<EmployeeDTO>> GetEmployeeById([FromRoute] long id) {
            return Ok(new ResultJson<EmployeeDTO> {
                Data = _employeeService.GetEmployeeById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Add new employee
        /// </summary>
        /// <param name="dto">Employee info for create</param>
        /// <returns>Employee added</returns>
        /// POST: api/employee
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultJson<EmployeeDTO>))]
        public ActionResult<ResultJson<EmployeeDTO>> AddNewEmployee([FromBody] RegisterDTO dto) {
            return Ok(new ResultJson<EmployeeDTO> {
                Data = _employeeService.AddNewEmployee(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }

        /// <summary>
        /// Update employee role
        /// </summary>
        /// <param name="dto">Employee role for update (only need id and isManager)</param>
        /// <returns>Employee updated</returns>
        /// PATCH: api/employee
        [HttpPatch]
        [Produces("application/json", Type = typeof(ResultJson<EmployeeDTO>))]
        public ActionResult<ResultJson<EmployeeDTO>> UpdateEmployeeRoleById([FromBody] EmployeeDTO dto) {
            return Ok(new ResultJson<EmployeeDTO> {
                Data = _employeeService.UpdateEmployeeRoleById(dto.Id, dto.IsManager),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Remove employee
        /// </summary>
        /// <param name="id">Id of employee want to delete</param>
        /// <returns>Delete status</returns>
        /// DELETE: api/employee
        [HttpDelete("{id:long}")]
        [Produces("application/json", Type = typeof(ResultJson<bool>))]
        public ActionResult<ResultJson<bool>> DeleteEmployeeById([FromRoute] long id) {
            return Ok(new ResultJson<bool> {
                Data = _employeeService.DeleteEmployeeById(id),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get list employee per page
        /// </summary>
        /// <param name="pageableModel">Pageable setting</param>
        /// <param name="dto">Search account model</param>
        /// <returns>Page content list employee</returns>
        /// GET: api/employee/pagination-filter
        [HttpGet("pagination-filter")]
        [Produces("application/json", Type = typeof(ResultJson<PageResponse<EmployeeDTO>>))]
        public ActionResult<ResultJson<PageResponse<EmployeeDTO>>> GetPageEmployee([FromQuery] PageableModel<SearchAccountDTO> pageableModel,
            [FromQuery] SearchAccountDTO dto) {
            pageableModel.SearchModel = dto;
            
            return Ok(new ResultJson<PageResponse<EmployeeDTO>> {
                Data = _employeeService.GetPageEmployee(pageableModel),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
    }
}