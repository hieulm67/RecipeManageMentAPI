using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JHipsterNet.Core.Pagination;
using JHipsterNet.Core.Pagination.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace RecipeManagementBE.Common.Repository.Impl
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly IUnitOfWork _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(IUnitOfWork context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual TEntity GetOne(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual IPage<TEntity> GetPage(IPageable pageable)
        {
            return _dbSet.UsePageable(pageable);
        }

        public virtual bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        public virtual TEntity Add(TEntity entity)
        {
            _dbSet.Add(entity);
            return entity;
        }

        public virtual bool AddRange(params TEntity[] entities)
        {
            _dbSet.AddRange(entities);
            return true;
        }

        public virtual TEntity Attach(TEntity entity)
        {
            var entry = _dbSet.Attach(entity);
            entry.State = EntityState.Added;
            return entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public virtual bool UpdateRange(params TEntity[] entities)
        {
            _dbSet.UpdateRange(entities);
            return true;
        }

        public virtual bool Clear()
        {
            var allEntities =  _dbSet.ToList();
            _dbSet.RemoveRange(allEntities);
            return true;
        }

        public virtual bool DeleteById(object id)
        {
            var entity = GetOne(id);
            _dbSet.Remove(entity);
            return true;
        }

        public virtual bool Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            return true;
        }

        public virtual int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public virtual int Count()
        {
            return _dbSet.Count();
        }

        public virtual IFluentRepository<TEntity> QueryHelper()
        {
            var fluentRepository = new FluentRepository<TEntity>(this, _dbSet);
            return fluentRepository;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public IDbContextTransaction BeginTransaction() {
            return _context.BeginTransaction();
        }
    }
}
