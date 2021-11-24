using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JHipsterNet.Core.Pagination;
using Microsoft.EntityFrameworkCore.Storage;

namespace RecipeManagementBE.Common.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity GetOne(object id);
        
        IEnumerable<TEntity> GetAll();
        
        IPage<TEntity> GetPage(IPageable pageable);
        
        bool Exists(Expression<Func<TEntity, bool>> predicate);
        
        int Count();
        
        bool DeleteById(object id);
        
        bool Delete(TEntity entity);
        
        bool Clear();
        
        int SaveChanges();
        
        TEntity Add(TEntity entity);
        
        bool AddRange(params TEntity[] entities);
        
        TEntity Attach(TEntity entity);
        
        TEntity Update(TEntity entity);
        
        bool UpdateRange(params TEntity[] entities);
        
        IFluentRepository<TEntity> QueryHelper();

        IDbContextTransaction BeginTransaction();
    }
}
