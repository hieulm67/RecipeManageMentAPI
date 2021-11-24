using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementBE.Constant;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Request.Authentication;
using RecipeManagementBE.Response;
using RecipeManagementBE.Service;

namespace RecipeManagementBE.Controllers {
    
    [ApiController]
    [Route(ApiPathURL.ACCOUNT_API_PATH)]
    [Authorize]
    public class AccountController : ControllerBase {

        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService) {
            _accountService = accountService;
        }

        /// <summary>
        /// Get all role available
        /// </summary>
        /// <returns>List of role</returns>
        /// GET: api/account/role
        [HttpGet("roles")]
        [Authorize(Policy = RoleConstants.ADMIN_CODE)]
        [Produces("application/json", Type = typeof(ResultJson<List<RoleDTO>>))]
        public ActionResult<ResultJson<List<RoleDTO>>> GetAllRole() {
            var token = HttpContext.GetTokenAsync("Bearer", "access_token");
            return Ok(new ResultJson<List<RoleDTO>> {
                Data = _accountService.GetAllRole(),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get current user information
        /// </summary>
        /// <returns>User info</returns>
        /// GET: api/account/my-info
        [HttpGet("my-info")]
        [Produces("application/json")]
        public ActionResult<ResultJson<object>> GetCurrentAccountInfo() {
            return Ok(new ResultJson<object> {
                Data = _accountService.GetCurrentAccountInfo(),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Update current account info
        /// </summary>
        /// <param name="dto">Account info for update</param>
        /// <returns>Account updated</returns>
        /// PUT: api/account
        [HttpPut]
        [Produces("application/json", Type = typeof(ResultJson<AccountDTO>))]
        public ActionResult<ResultJson<AccountDTO>> UpdateAccountById([FromBody] AccountDTO dto) {
            return Ok(new ResultJson<AccountDTO> {
                Data = _accountService.UpdateAccountInfo(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Update account password by uid and email
        /// </summary>
        /// <param name="dto">Account info for update</param>
        /// <returns>Account updated</returns>
        /// PATCH: api/account
        [HttpPatch]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultJson<AccountDTO>))]
        public ActionResult<ResultJson<AccountDTO>> UpdateAccountPasswordByUIDAndEmail([FromBody] AccountDTO dto) {
            return Ok(new ResultJson<AccountDTO> {
                Data = _accountService.UpdateAccountPasswordByUIDAndEmail(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Check current account password
        /// </summary>
        /// <param name="password">Password for checking</param>
        /// <returns>Password match or not</returns>
        /// PUT: api/account
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultJson<bool>))]
        public ActionResult<ResultJson<bool>> CheckPasswordCurrentAccount([FromBody] string password) {
            return Ok(new ResultJson<bool> {
                Data = _accountService.CheckPasswordCurrentAccount(password),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
    }
}