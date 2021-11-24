using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class RecipeToolRepository : GenericRepository<RecipeTool>, IRecipeToolRepository {
        public RecipeToolRepository(IUnitOfWork context) : base(context) {
        }
    }
}