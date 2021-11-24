using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementBE.Constant;
using RecipeManagementBE.Request.Authentication;
using RecipeManagementBE.Response;
using RecipeManagementBE.Service;

namespace RecipeManagementBE.Controllers {
    
    [Route(ApiPathURL.API_V1_PATH)]
    [ApiController]
    public class AuthenticationController : ControllerBase {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService) {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Authenticate with login dto
        /// </summary>
        /// <param name="dto">Login info for authentication</param>
        /// <returns>JWT access token, expire time of token, refresh token</returns>
        /// POST: api/authenticate
        [HttpPost("authenticate")]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultJson<AuthenticationResult>))]
        public ActionResult<ResultJson<AuthenticationResult>> Authenticate([FromBody] LoginDTO dto) {
            return Ok(new ResultJson<AuthenticationResult> {
                Data = _authenticationService.Authenticate(dto, false),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Refresh token with expiry access token and refresh token
        /// </summary>
        /// <param name="dto">Token info for refreshing</param>
        /// <returns>JJWT access token, expire time of token, refresh token</returns>
        /// POST: api/refresh-token
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultJson<AuthenticationResult>))]
        public ActionResult<ResultJson<AuthenticationResult>> Authenticate([FromBody] TokenRequest dto) {
            return Ok(new ResultJson<AuthenticationResult> {
                Data = _authenticationService.RefreshToken(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
    }
}