using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class AccountRepository : GenericRepository<Account>, IAccountRepository {
        
        public AccountRepository(IUnitOfWork context) : base(context) {
        }
    }
}