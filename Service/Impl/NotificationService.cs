using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using JHipsterNet.Core.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RecipeManagementBE.Constant;
using RecipeManagementBE.DTO;
using RecipeManagementBE.Entity;
using RecipeManagementBE.Repository;
using RecipeManagementBE.Request;
using RecipeManagementBE.Response;
using RecipeManagementBE.Util;

namespace RecipeManagementBE.Service.Impl {
    public class NotificationService : BaseService, INotificationService {

        private readonly INotificationRepository _notificationRepository;
        
        private readonly ILogger<NotificationService> _logger;

        private readonly IMapper _mapper;

        private const string NOTIFICATION_SENDING_TIME = "sendingTime";

        public NotificationService(IHttpContextAccessor httpContextAccessor, INotificationRepository notificationRepository,
            ILogger<NotificationService> logger, IMapper mapper, IAccountRepository accountRepository) : base(httpContextAccessor, accountRepository) {
            _notificationRepository = notificationRepository;
            _logger = logger;
            _mapper = mapper;
        }


        public List<NotificationDTO> GetAllNotification() {
            
            var currentAccountUID = GetCurrentAccountUID();

            if (string.IsNullOrWhiteSpace(currentAccountUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw" , DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            var notificationEntities = _notificationRepository.QueryHelper()
                .Filter(notification => notification.To.Equals(currentAccountUID))
                .OrderBy(notifications => notifications.OrderByDescending(notification => notification.SendingTime))
                .GetAll().ToList();
            return _mapper.Map<List<NotificationDTO>>(notificationEntities);
        }
        
        public PageResponse<NotificationDTO> GetPageNotification(PageableModel<DateTime> pageableModel) {

            var sendingTime = pageableModel.SearchModel;
            
            var sortField = pageableModel.SortField ?? NOTIFICATION_SENDING_TIME;

            var sort = pageableModel.SortDirection <= 0
                ? new Sort(Direction.Desc, sortField)
                : new Sort(Direction.Asc, sortField);
            
            var currentAccountUID = GetCurrentAccountUID();

            if (string.IsNullOrWhiteSpace(currentAccountUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw" , DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            Expression<Func<Notification, bool>> filter = notification => notification.To.Equals(currentAccountUID);

            if (sendingTime.Year != 1) {
                Expression<Func<Notification, bool>> filterTime = notification => TimeSpan.Compare(notification.SendingTime.TimeOfDay, sendingTime.TimeOfDay) == 0;
                filter = filter.And(filterTime);
            }

            var notificationEntities = _notificationRepository.QueryHelper()
                .Filter(filter)
                .GetPage(Pageable.Of(pageableModel.PageNumber, pageableModel.PageSize, sort));
            
            return _mapper.Map<PageResponse<NotificationDTO>>(notificationEntities);
        }

        public bool AddNewNotification(string content, Account toAccount, string type, long notifiedId) {

            if (toAccount == null) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing to account entity, missing required field exception throw" , DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowMissingField(new[] {"toAccount"});
            }

            var newEntity = new Notification {
                ToAccount = toAccount,
                To = toAccount?.UID, 
                PayloadContent = content,
                Type = type,
                NotifiedId = notifiedId
            };

            _notificationRepository.Add(newEntity);
            
            return true;
        }

        public bool MarkAllAsRead() {

            var currentAccountUID = GetCurrentAccountUID();

            if (string.IsNullOrWhiteSpace(currentAccountUID)) {
                _logger.LogError("[{Time}] [{ApplicationName}]: Missing current account uid, unauthorized exception throw" , DateTime.Now ,Constants.APPLICATION_NAME);
                ThrowUnauthorizedException(new[] {"uid"});
            }

            var notificationEntities = _notificationRepository.QueryHelper()
                .Filter(notification => notification.To.Equals(currentAccountUID))
                .GetAll().ToList();

            notificationEntities.ForEach(notification => notification.IsSeen = true);

            _notificationRepository.UpdateRange(notificationEntities.ToArray());
            _notificationRepository.SaveChanges();
            
            return true;
        }
    }
}