using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace Repository.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<T> InsertAsync(T entity);
        Task<ICollection<T>> InsertManyAsync(ICollection<T> entities);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);

        Task<E?> Get<E>(
            Expression<Func<T, E>> selector,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        Task<IEnumerable<E>> GetAllAsync<E>(
            Expression<Func<T, E>> selector,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
    }
}