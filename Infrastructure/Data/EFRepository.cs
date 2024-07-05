using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    internal class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected EleganceParadisContext DbContext;
        protected DbSet<TEntity> DbSet;

        public EFRepository(EleganceParadisContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
        }

        public TEntity Add(TEntity entity)
        {
            DbSet.Add(entity);
            //DbContext.Set<TEntity>().Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            DbSet.Add(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
            DbContext.SaveChanges();
            return entities;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
            await DbContext.SaveChangesAsync();
            return entities;
        }

        public bool Any(Expression<Func<TEntity, bool>> expression)
        {
            return DbSet.Any(expression);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await DbSet.AnyAsync(expression);
        }

        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            DbSet.Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
            DbContext.SaveChanges();
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
            await DbContext.SaveChangesAsync();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            return DbSet.FirstOrDefault(expression)!;
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        {
            return (await DbSet.FirstOrDefaultAsync(expression))!;
        }

        public TEntity GetById<TId>(TId id)
        {
            return DbSet.Find(new object[] { id })!;
        }

        public async Task<TEntity> GetByIdAsync<TId>(TId id)
        {
            return (await DbSet.FindAsync(new object[] { id }))!;
        }

        public List<TEntity> List(Expression<Func<TEntity, bool>> expression)
        {
            return DbSet.Where(expression).ToList();
        }

        public async Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await DbSet.Where(expression).ToListAsync();
        }

        public TEntity Update(TEntity entity)
        {
            DbSet.Update(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            DbSet.Update(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
            DbContext.SaveChanges();
            return entities;
        }

        public async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
            await DbContext.SaveChangesAsync();
            return entities;
        }
    }
}
