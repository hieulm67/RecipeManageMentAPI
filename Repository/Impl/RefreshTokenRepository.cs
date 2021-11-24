using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository {
        public RefreshTokenRepository(IUnitOfWork context) : base(context) {
        }
    }
}