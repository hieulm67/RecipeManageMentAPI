using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class DishRepository : GenericRepository<Dish>, IDishRepository {
        public DishRepository(IUnitOfWork context) : base(context) {
        }
    }
}