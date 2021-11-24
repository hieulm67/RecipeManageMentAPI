using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class RoleRepository : GenericRepository<Role>, IRoleRepository {
        
        public RoleRepository(IUnitOfWork context) : base(context) {
        }
    }
}