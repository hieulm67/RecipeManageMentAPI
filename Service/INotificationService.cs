using System;
using System.Collections.Generic;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Request;
using RecipeManagementBE.Response;

namespace RecipeManagementBE.Service {
    public interface INotificationService {

        List<NotificationDTO> GetAllNotification();

        PageResponse<NotificationDTO> GetPageNotification(PageableModel<DateTime> pageableModel);

        bool AddNewNotification(string content, Account toAccount, string type, long notifiedId);

        bool MarkAllAsRead();
    }
}