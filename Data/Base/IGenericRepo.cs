using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace Data.Base
{
    public interface IGenericRepo<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task Update(T entity);
        Task Remove(T entity);
        void UpdateE(T entity);
        Task<T?> GetSingleOrDefaultWithNoTracking(Expression<Func<T, bool>>? func = null);
        Task<List<T>> GetListWithNoTracking(Expression<Func<T, bool>>? func = null);

    }
}
