using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class IngredientRepository : GenericRepository<Ingredient>, IIngredientRepository {
        public IngredientRepository(IUnitOfWork context) : base(context) {
        }
    }
}