using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository {
        public CategoryRepository(IUnitOfWork context) : base(context) {
        }
    }
}