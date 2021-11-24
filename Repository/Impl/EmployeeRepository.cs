using RecipeManagementBE.Common.Repository;
using RecipeManagementBE.Common.Repository.Impl;
using RecipeManagementBE.Entity;

namespace RecipeManagementBE.Repository.Impl {
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository {
        public EmployeeRepository(IUnitOfWork context) : base(context) {
        }
    }
}