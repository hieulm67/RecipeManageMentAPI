using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace RecipeManagementBE.Common.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        DbSet<T> Set<T>(string name = null) where T : class;
        int SaveChanges();
        void AddOrUpdateGraph<TEntiy>(TEntiy entity) where TEntiy : class;
        void UpdateState<TEntity>(TEntity entity, EntityState state);
        void SetEntityStateModified<TEntiy, TProperty>(TEntiy entity, Expression<Func<TEntiy, TProperty>> propertyExpression) where TEntiy : class where TProperty : class;
        IDbContextTransaction BeginTransaction();
    }
}
