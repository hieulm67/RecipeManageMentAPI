using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class QARepository : GenericRepository<Qa>, IQARepository {
        public QARepository(IUnitOfWork context) : base(context) {
        }
    }
}