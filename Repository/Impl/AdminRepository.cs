using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class AdminRepository : GenericRepository<Admin>, IAdminRepository {
        public AdminRepository(IUnitOfWork context) : base(context) {
        }
    }
}