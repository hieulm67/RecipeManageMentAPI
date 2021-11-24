using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class RecipeDetailRepository : GenericRepository<RecipeDetail>, IRecipeDetailRepository {
        public RecipeDetailRepository(IUnitOfWork context) : base(context) {
        }
    }
}