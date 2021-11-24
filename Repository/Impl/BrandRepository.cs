using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository {
        
        public BrandRepository(IUnitOfWork context) : base(context) {
        }
    }
}