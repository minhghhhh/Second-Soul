using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Base
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        protected DbSet<T> _dbSet;
        protected readonly SecondSoulShopContext context;
        public GenericRepo(SecondSoulShopContext context)
        {
            this.context = context;
            _dbSet = context.Set<T>();
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            _ = await _dbSet.AddAsync(entity);
            _ = await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _ =  _dbSet.Update(entity);
            _ = await context.SaveChangesAsync();
        }
        public async Task<int> Update(T entity)
        {
            _dbSet.Update(entity);
            return await context.SaveChangesAsync();
        }
        public async Task<int> Delete(T entity)
        {
            _dbSet.Remove(entity);
            return await context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task Remove(T entity)
        {
            _ = _dbSet.Remove(entity);
            _ = await context.SaveChangesAsync();
        }

        public void UpdateE(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<T?> GetSingleOrDefaultWithNoTracking(Expression<Func<T, bool>>? func = null)
        {
            var predicate = func ?? (_dbSet => false);
            return await _dbSet.AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetListWithNoTracking(Expression<Func<T, bool>>? func = null)
        {
            var predicate = func ?? (_dbSet => true);
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

      public async Task<int> CreateAsync(T entity)
        {
            _dbSet.Add(entity);
            return await context.SaveChangesAsync();
        }

        Task IGenericRepo<T>.Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
