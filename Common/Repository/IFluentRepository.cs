using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using Microsoft.EntityFrameworkCore.Query;

namespace RecipeManagementBE.Common.Repository
{
    public interface IFluentRepository<TEntity> where TEntity : class
    {
        IFluentRepository<TEntity> Filter(Expression<Func<TEntity, bool>> filter);
        
        IFluentRepository<TEntity> Include(Expression<Func<TEntity, object>> expression);
        
        IFluentRepository<TEntity> Include(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include);
        
        IFluentRepository<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        
        IFluentRepository<TEntity> AsNoTracking();
        
        TEntity GetOne(Expression<Func<TEntity, bool>> filter);
        
        IEnumerable<TEntity> GetAll();
        
        IPage<TEntity> GetPage(IPageable pageable);
    }
}
