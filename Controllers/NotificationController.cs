using System;
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
    [Route(ApiPathURL.NOTIFICATION_API_PATH)]
    [Authorize(Policy = RoleConstants.EMPLOYEE_CODE)]
    public class NotificationController : ControllerBase {

        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService) {
            _notificationService = notificationService;
        }
        
        /// <summary>
        /// Get all notification for current account
        /// </summary>
        /// <returns>List of notification</returns>
        /// GET: api/notifications
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultJson<List<NotificationDTO>>))]
        public ActionResult<ResultJson<List<NotificationDTO>>> GetAllNotification() {
            return Ok(new ResultJson<List<NotificationDTO>> {
                Data = _notificationService.GetAllNotification(),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Get page notification for current account
        /// </summary>
        /// <remarks>Search model is qaTime</remarks>
        /// <returns>List of notification</returns>
        /// GET: api/notifications
        [HttpGet("pagination-filter")]
        [Produces("application/json", Type = typeof(ResultJson<PageResponse<NotificationDTO>>))]
        public ActionResult<ResultJson<PageResponse<NotificationDTO>>> GetPageNotification([FromQuery] PageableModel<DateTime> pageableModel) {
            return Ok(new ResultJson<PageResponse<NotificationDTO>> {
                Data = _notificationService.GetPageNotification(pageableModel),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
        
        /// <summary>
        /// Update all notification as read 
        /// </summary>
        /// <returns>Update status</returns>
        /// PUT: api/notifications
        [HttpPut]
        [Produces("application/json", Type = typeof(ResultJson<bool>))]
        public ActionResult<ResultJson<bool>> MarkAllNotificationAsRead() {
            return Ok(new ResultJson<bool> {
                Data = _notificationService.MarkAllAsRead(),
                Message = Constants.SUCCESS_MESSAGE_RESPONSE
            });
        }
    }
}