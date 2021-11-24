using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository {
        public RecipeRepository(IUnitOfWork context) : base(context) {
        }
    }
}