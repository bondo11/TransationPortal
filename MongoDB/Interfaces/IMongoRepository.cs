using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using MongoDB.Driver;

using translate_spa.Models.Interfaces;

namespace translate_spa.MongoDB.Interfaces
{
    public interface IMongoRepository<T> where T : ITranslation
    {
        IEnumerable<T> All();

        Task<IEnumerable<T>> AllAsync();

        bool Any();

        bool Any(Expression<Func<T, bool>> where);

        IEnumerable<T> Query(Expression<Func<T, bool>> where);

        Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> where);

        Task<bool> AnyAsync();

        Task<bool> AnyAsync(Expression<Func<T, bool>> where);

        Task AddAsync(T entity);
        void Add(T entity);

        Task AddManyAsync(ICollection<T> entities);
        void AddMany(ICollection<T> entities);

        void Delete(object key);
        void Delete(T entity);

        void Delete(Expression<Func<T, bool>> where);

        Task DeleteAsync(object key);

        Task DeleteAsync(T entity);

        Task DeleteAsync(Expression<Func<T, bool>> where);

        void Update(T item);

        void Update(Expression<Func<T, bool>> where, T item);

        Task UpdateAsync(T item);

        Task UpdateAsync(Expression<Func<T, bool>> where, T item);

        int Count(Expression<Func<T, bool>> filter);

        Task<int> CountAsync(Expression<Func<T, bool>> filter);

        T FirstOrDefault(Expression<Func<T, bool>> where);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where);

        T SingleOrDefault(Expression<Func<T, bool>> where);

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where);

        T First(Expression<Func<T, bool>> where);

        Task<T> FirstAsync(Expression<Func<T, bool>> where);

        T Single(Expression<Func<T, bool>> where);

        Task<T> SingleAsync(Expression<Func<T, bool>> where);

        Task<ICollection<T>> GetByPageAndQuantityAsync(
            int page,
            int quantity,
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null);

        ICollection<T> GetByPageAndQuantity(
            int page,
            int quantity,
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null);
    }
}