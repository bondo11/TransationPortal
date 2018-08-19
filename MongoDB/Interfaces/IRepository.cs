using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using MongoDB.Driver;

using translate_spa.Models.Interfaces;

namespace translate_spa.MongoDB.Interfaces
{
    public interface IRepository<T> where T : ITranslation
    {
        bool AnySync();

        bool AnySync(Expression<Func<T, bool>> where);

        Task<bool> Any();

        Task<bool> Any(Expression<Func<T, bool>> where);

        Task Add(T entity);
        void AddSync(T entity);

        Task AddManyAsync(ICollection<T> entities);
        void AddManySync(ICollection<T> entities);

        void DeleteSync(object key);

        void DeleteSync(Expression<Func<T, bool>> where);

        Task Delete(object key);

        Task Delete(Expression<Func<T, bool>> where);

        Task ReplaceOne(T entity, T newValue);
        void ReplaceOneSync(Expression<Func<T, bool>> filter, T newValue);

        Task BulkInsert(ICollection<T> entities);
        void BulkInsertSync(ICollection<T> entities);

        Task<long> Count(Expression<Func<T, bool>> filter);
        long CountSync(Expression<Func<T, bool>> filter);

        Task<ICollection<T>> GetByExpression(
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null);

        ICollection<T> GetByExpressionSync(
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null);

        Task<ICollection<T>> GetAll(SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null);

        ICollection<T> GetAllSync(SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null);

        Task<ICollection<T>> GetByPageAndQuantity(
            int page,
            int quantity,
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null);

        ICollection<T> GetByPageAndQuantitySync(
            int page,
            int quantity,
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null);

        T FirstOrDefaultSync(Expression<Func<T, bool>> where);

        Task<T> FirstOrDefault(Expression<Func<T, bool>> where);

        void UpdateSync(T item, object key);

        Task Update(T item, object key);

        T SingleOrDefaultSync(Expression<Func<T, bool>> where);

        Task<T> SingleOrDefault(Expression<Func<T, bool>> where);

        Task<T> GetSingleByExpression(
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null,
            int index = 1);

        T GetSingleByExpressionSync(
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null,
            int index = 1);

    }
}