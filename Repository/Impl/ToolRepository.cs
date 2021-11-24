using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class ToolRepository : GenericRepository<Tool>, IToolRepository {
        public ToolRepository(IUnitOfWork context) : base(context) {
        }
    }
}