using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

using NLog;

using translate_spa.Models;
using translate_spa.Models.Interfaces;
using translate_spa.MongoDB.Interfaces;

namespace translate_spa.Repositories
{
    public class MongoRepository<T> : IMongoRepository<T> where T : class, ITranslation
    {
        #region Class Initialization
        protected IFindFluent<T, T> BuildFindFluent(
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null)
        {
            var fluent = MongoCollection.Find(filter);
            if (projection != null)
            {
                fluent.Options.Projection = Builders<T>.Projection.Combine(projection);
            }

            if (sort != null)
            {
                fluent.Sort(sort);
            }

            return fluent;
        }

        private static FilterDefinition<T> FilterId(object key)
        {
            return Builders<T>.Filter.Eq("Id", key);
        }
        #endregion
        #region Query functions
        readonly ILogger _log = new NLog.LogFactory().GetCurrentClassLogger();

        public MongoRepository(IDbBuilder dbBuilder, IMongoDatabase mongoDatabase)
        {
            this.DbBuilder = dbBuilder;
            this.MongoDatabase = mongoDatabase;

        }
        protected IDbBuilder DbBuilder { get; }

        protected IMongoCollection<T> MongoCollection { get; set; }

        protected IMongoDatabase MongoDatabase { get; set; }

        public MongoRepository(IDbBuilder builder)
        {
            DbBuilder = builder;
            Initialize();
        }

        protected void Initialize()
        {
            GetDatabase();
            GetCollection();
            if (MongoCollection == null)return;
            _log.Debug("Creating mongo index");
            CreateIndex();
        }

        protected void GetDatabase()
        {
            var client = new MongoClient(DbBuilder.GetConnectionString());
            MongoDatabase = client.GetDatabase(DbBuilder.GetDatabaseName());
        }

        protected void GetCollection()
        {
            var type = typeof(T);
            if (MongoCollection != null)return;
            _log.Debug($"Getting mongocollection {typeof(T)}");
            MongoCollection = MongoDatabase.GetCollection<T>(type.Name);
        }

        protected virtual void CreateIndex()
        {
            try
            {
                var indexDefinition = Builders<T>.IndexKeys.Combine(
                    Builders<T>.IndexKeys.Ascending(x => x.Id),
                    Builders<T>.IndexKeys.Ascending(x => x.Key),
                    Builders<T>.IndexKeys.Ascending(x => x.Branch),
                    Builders<T>.IndexKeys.Ascending(x => x.Environment)
                );

                MongoCollection.Indexes.CreateOne(indexDefinition);
            }
            catch (Exception exception)
            {
                _log.Error($"Error creating index {typeof(T)}");
                _log.Error(exception.StackTrace);
            }
        }
        #endregion
        #region QueryBuilders

        public IEnumerable<T> All()
        {
            var findFluent = BuildFindFluent(_ => true, null, null);
            return findFluent.ToList();
        }

        public async Task<IEnumerable<T>> AllAsync()
        {
            var findFluent = BuildFindFluent(_ => true, null, null);
            return await findFluent.ToListAsync();
        }

        public IEnumerable<T> Query(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            return MongoCollection.AsQueryable().Where(filter);
        }

        public async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            return MongoCollection.AsQueryable().Where(filter);
        }

        public bool Any()
        {
            return MongoCollection.AsQueryable().Any();
        }

        public bool Any(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            return MongoCollection.AsQueryable().Any(filter);
        }

        public async Task<bool> AnyAsync()
        {
            return await MongoCollection.AsQueryable().AnyAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            return MongoCollection.AsQueryable().Where(filter).Any();
        }

        public async Task AddAsync(T entity)
        {
            await MongoCollection.InsertOneAsync(entity);
        }

        public void Add(T entity)
        {
            MongoCollection.InsertOne(entity);
        }

        public async Task AddManyAsync(ICollection<T> entities)
        {
            await MongoCollection.InsertManyAsync(entities);
        }

        public void AddMany(ICollection<T> entities)
        {
            MongoCollection.InsertMany(entities);
        }

        public void Delete(object key)
        {
            MongoCollection.DeleteOne(FilterId(key));
        }
        public void Delete(T entity)
        {
            MongoCollection.DeleteOne(FilterId(entity.Id));
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            MongoCollection.DeleteOne(where);
        }

        public async Task DeleteAsync(object key)
        {
            await MongoCollection.DeleteOneAsync(FilterId(key));
        }

        public async Task DeleteAsync(T entity)
        {
            await MongoCollection.DeleteOneAsync(FilterId(entity.Id));
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> where)
        {
            await MongoCollection.DeleteOneAsync(where);
        }

        public void Update(T item)
        {
            MongoCollection.ReplaceOne(FilterId(item.Id), item);
        }

        public async Task UpdateAsync(T item)
        {
            await MongoCollection.ReplaceOneAsync(FilterId(item.Id), item);
        }

        public void Update(Expression<Func<T, bool>> where, T item)
        {
            var options = new UpdateOptions();
            options.IsUpsert = true;
            var updateDefinition = Builders<T>.Update
                .Set(x => x.Key, item.Key)
                .Set(x => x.Da, item.Da)
                .Set(x => x.En, item.En)
                .Set(x => x.Sv, item.Sv)
                .Set(x => x.Nb, item.Nb)
                .Set(x => x.Environment, item.Environment)
                .Set(x => x.Branch, item.Branch)
                .SetOnInsert(x => x.Id, ObjectId.GenerateNewId().ToString());
            /* foreach (var property in item.GetProperties())
            {
                updateDefinition = updateDefinition.Set(x => property.Key, property.Value);
            } */

            var def = Builders<T>.Filter.Where(where);
            var result = MongoCollection.UpdateOne(def, updateDefinition, options);
            _log.Debug($"Updated item '{item.Key}' with result '{result.ToString()}'");
        }

        public async Task UpdateAsync(Expression<Func<T, bool>> where, T item)
        {
            var options = new UpdateOptions();
            options.IsUpsert = true;
            var updateDefinition = Builders<T>.Update
                /* .Set(x => x.Key, item.Key)
                .Set(x => x.Da, item.Da)
                .Set(x => x.En, item.En)
                .Set(x => x.Sv, item.Sv)
                .Set(x => x.Nb, item.Nb)
                .Set(x => x.Environment, item.Environment)
                .Set(x => x.Branch, item.Branch) */
                .SetOnInsert(x => x.Id, ObjectId.GenerateNewId().ToString());
            foreach (var property in item.GetProperties())
            {
                updateDefinition = updateDefinition.Set(x => property.Key, property.Value ?? string.Empty);
            }

            var result = await MongoCollection.UpdateOneAsync(where, updateDefinition, options);
            _log.Debug($"Updated item '{item.Key}' with result '{result.ToString()}'");
        }

        public int Count(Expression<Func<T, bool>> filter)
        {
            return unchecked((int)MongoCollection.CountDocuments(filter));
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
        {
            return unchecked((int)await MongoCollection.CountDocumentsAsync(filter));
        }

        public T FirstOrDefault(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            return MongoCollection.AsQueryable().FirstOrDefault(filter);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            return MongoCollection.AsQueryable().FirstOrDefault(filter);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            try
            {
                return MongoCollection.AsQueryable().SingleOrDefault(filter);
            }
            catch (System.Exception exception)
            {
                _log.Error("Trying to make a single query, but multible instances found,");
                throw exception;
            }
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            try
            {
                return MongoCollection.AsQueryable().SingleOrDefault(filter);
            }
            catch (System.Exception exception)
            {
                _log.Error("Trying to make a single query, but multible instances found,");
                throw exception;
            }
        }

        public async Task<T> FirstAsync(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            try
            {
                return MongoCollection.AsQueryable().First(filter);
            }
            catch (System.Exception exception)
            {
                _log.Error("None found, you have to have at least one document result on your query,");
                throw exception;
            }
        }

        public T First(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            try
            {
                return MongoCollection.AsQueryable().First(filter);
            }
            catch (System.Exception exception)
            {
                _log.Error("None found, you have to have at least one document result on your query,");
                throw exception;
            }
        }

        public T Single(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            try
            {
                return MongoCollection.AsQueryable().First(filter);
            }
            catch (System.Exception exception)
            {
                _log.Error("Multible or none found, you have to have at exactly one document result on your query,");
                throw exception;
            }
        }

        public async Task<T> SingleAsync(Expression<Func<T, bool>> where)
        {
            var filter = where.Compile();
            try
            {
                return MongoCollection.AsQueryable().First(filter);
            }
            catch (System.Exception exception)
            {
                _log.Error("Multible or none found, you have to have at exactly one document result on your query,");
                throw exception;
            }
        }

        public async Task<ICollection<T>> GetByPageAndQuantityAsync(
            int page,
            int quantity,
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null)
        {
            var findFluent = BuildFindFluent(filter, sort, projection);
            return await findFluent.Limit(quantity).Skip((page - 1) * quantity).ToListAsync();
        }

        public ICollection<T> GetByPageAndQuantity(
            int page,
            int quantity,
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null)
        {
            var findFluent = BuildFindFluent(filter, sort, projection);
            return findFluent.Limit(quantity).Skip((page - 1) * quantity).ToList();
        }

        #endregion
    }
}