using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementBE.Constant;
using RecipeManagementBE.Request.Create;
using RecipeManagementBE.Response;
using RecipeManagementBE.Service;

namespace RecipeManagementBE.Controllers {
    
    [ApiController]
    [Route("/api/v1/[controller]")]
    [Authorize(Policy = RoleConstants.ADMIN_MANAGER_CODE)]
    public class ImageController {
        
        private readonly IFirebaseService _firebaseService;

        public ImageController(IFirebaseService firebaseService) {
            _firebaseService = firebaseService;
        }

        [HttpPost]
        public async Task<ActionResult<ResultJson<string>>> UploadImage([FromBody] UploadImageDTO dto) {
            return new ResultJson<string> {
                Data = await _firebaseService.UploadImage(dto),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            };
        }
    }
}