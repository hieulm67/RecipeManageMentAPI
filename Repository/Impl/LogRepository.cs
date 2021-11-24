using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class LogRepository : GenericRepository<Log>, ILogRepository {
        public LogRepository(IUnitOfWork context) : base(context) {
        }
    }
}