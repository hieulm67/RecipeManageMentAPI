using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository {
        public NotificationRepository(IUnitOfWork context) : base(context) {
        }
    }
}