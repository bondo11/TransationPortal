using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

using translate_spa.Models.Interfaces;
using translate_spa.MongoDB.Interfaces;

namespace translate_spa.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class, IEntity
    {
        public BaseRepository(IDbBuilder dbBuilder, IMongoDatabase mongoDatabase)
        {
            this.DbBuilder = dbBuilder;
            this.MongoDatabase = mongoDatabase;

        }
        protected IDbBuilder DbBuilder { get; }

        protected IMongoCollection<T> MongoCollection { get; set; }

        protected IMongoDatabase MongoDatabase { get; set; }

        public BaseRepository(IDbBuilder builder)
        {
            DbBuilder = builder;
            Initialize();
        }

        protected void Initialize()
        {
            GetDatabase();
            GetCollection();
            if (MongoCollection == null)return;
            Console.WriteLine("Creating mongo index");
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
            Console.WriteLine($"Getting mongocollection {typeof(T)}");
            MongoCollection = MongoDatabase.GetCollection<T>(type.Name);
        }

        protected virtual void CreateIndex()
        {
            try
            {
                /* Deprecated 
                MongoCollection.Indexes.CreateOne(
                   Builders<T>.IndexKeys
                   .Descending(x => x.Id)
                   .Descending(x => x.Key)
                   .Descending(x => x.Branch)
                   .Descending(x => x.En)
                   .Descending(x => x.Da),
                   new CreateIndexOptions { Name = "translations_id" }); */

                /* Deprecated 
                var indexDefinition = Builders<T>.IndexKeys.Combine(
                    Builders<T>.IndexKeys.Ascending(f => f.Key),
                    Builders<T>.IndexKeys.Ascending(f => f.Da),
                    Builders<T>.IndexKeys.Ascending(f => f.En),
                    Builders<T>.IndexKeys.Ascending(f => f.Sv),
                    Builders<T>.IndexKeys.Ascending(f => f.Nb),
                    Builders<T>.IndexKeys.Ascending(f => f.Branch));

                MongoCollection.Indexes.CreateOne(indexDefinition);
                */

                var indexBuilder = Builders<T>.IndexKeys;
                var indexModel = new CreateIndexModel<T>(indexBuilder.Ascending(x => x.Key));

                MongoCollection.Indexes.CreateOne(indexModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error creating index {typeof(T)}");
                Console.WriteLine(exception.StackTrace);
            }
        }

        public bool AnySync()
        {
            return MongoCollection.Find(new BsonDocument()).Any();
        }

        public bool AnySync(Expression<Func<T, bool>> where)
        {
            return MongoCollection.Find(where).Any();
        }

        public Task<bool> Any()
        {
            return MongoCollection.Find(new BsonDocument()).AnyAsync();
        }

        public Task<bool> Any(Expression<Func<T, bool>> where)
        {
            return MongoCollection.Find(where).AnyAsync();
        }

        public void AddSync(T entity)
        {
            MongoCollection.InsertOne(entity);
        }

        public async Task Add(T entity)
        {
            await MongoCollection.InsertOneAsync(entity);
        }

        public async Task AddMany(ICollection<T> entities)
        {
            await MongoCollection.InsertManyAsync(entities);
        }

        public void AddManySync(ICollection<T> entities)
        {
            MongoCollection.InsertMany(entities);
        }

        public async Task BulkInsert(ICollection<T> entities)
        {
            var stores = new List<WriteModel<T>>();

            stores.AddRange(entities.Select(x => new InsertOneModel<T>(x)));

            await MongoCollection.BulkWriteAsync(stores);
        }

        public void BulkInsertSync(ICollection<T> entities)
        {
            var stores = new List<WriteModel<T>>();

            stores.AddRange(entities.Select(x => new InsertOneModel<T>(x)));

            MongoCollection.BulkWrite(stores);
        }

        public async Task<long> Count(Expression<Func<T, bool>> filter)
        {
            return await MongoCollection.CountDocumentsAsync(filter);
        }

        public long CountSync(Expression<Func<T, bool>> filter)
        {
            return MongoCollection.CountDocuments(filter);
        }

        public void DeleteSync(object key)
        {
            MongoCollection.DeleteOne(FilterId(key));
        }

        public void DeleteSync(T entity)
        {
            MongoCollection.DeleteOne(FilterId(entity.Id));
        }

        public void DeleteSync(Expression<Func<T, bool>> where)
        {
            MongoCollection.DeleteMany(where);
        }

        public void DeleteOneSync(Expression<Func<T, bool>> where)
        {
            MongoCollection.DeleteOne(where);
        }

        public Task Delete(object key)
        {
            return MongoCollection.DeleteOneAsync(FilterId(key));
        }

        public Task Delete(T entity)
        {
            return MongoCollection.DeleteOneAsync(FilterId(entity.Id));
        }

        public Task DeleteOne(Expression<Func<T, bool>> where)
        {
            return MongoCollection.DeleteOneAsync(where);
        }

        public Task Delete(Expression<Func<T, bool>> where)
        {
            return MongoCollection.DeleteManyAsync(where);
        }

        public void UpdateSync(Expression<Func<T, bool>> where, T item)
        {
            var Update = Builders<T>.Update.Unset(x => x.Id);
            foreach (var property in item.GetProperties())
            {
                Update.SetOnInsert(x => property.Key, property.Value);
                Update.Set(x => property.Key, property.Value);
            }

            MongoCollection.UpdateOne(where, Update);
        }

        /*     public Task Update(Expression<Func<T, bool>> where, T item)
            {
                var Update = Builders<T>.Update.Unset(x => "Id");
                foreach (var property in item.GetProperties())
                {
                    Update.SetOnInsert(x => property.Key, property.Value);
                    Update.Set(x => property.Key, property.Value);
                }
                var options = new UpdateOptions
                {
                    IsUpsert = true,
                };

                return MongoCollection.UpdateOneAsync(where, Update, options);
            } */

        public Task Update(Expression<Func<T, bool>> where, T item)
        {
            return MongoCollection.ReplaceOneAsync(where, item);
        }

        public void UpdateSync(T item, object key)
        {
            MongoCollection.ReplaceOne(FilterId(key), item);
        }

        public Task Update(T item, object key)
        {
            return MongoCollection.ReplaceOneAsync(FilterId(key), item);
        }

        public async Task ReplaceOne(T entity, T newValue)
        {
            try
            {
                await MongoCollection.ReplaceOneAsync(FilterId(entity.Id), newValue);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
            }
        }

        public async Task ReplaceOne(Expression<Func<T, bool>> filter, T newValue)
        {
            try
            {
                await MongoCollection.FindOneAndReplaceAsync(filter: filter, replacement: newValue);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
            }
        }

        public void ReplaceOneSync(Expression<Func<T, bool>> filter, T newValue)
        {
            try
            {
                MongoCollection.ReplaceOne(filter, newValue);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error creating index {typeof(T)}");
                Console.WriteLine(exception.StackTrace);
            }
        }

        public ICollection<T> GetByExpressionSync(Expression<Func<T, bool>> filter, SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null)
        {
            var findFluent = BuildFindFluent(filter, sort, projection);
            return findFluent.ToList();
        }

        public async Task<ICollection<T>> GetByExpression(Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            var findFluent = BuildFindFluent(filter, sort, projection);
            return await findFluent.ToListAsync();
        }

        public async Task<ICollection<T>> GetAll(SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null)
        {
            var findFluent = BuildFindFluent(_ => true, sort, projection);
            return await findFluent.ToListAsync();
        }

        public ICollection<T> GetAllSync(SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            var findFluent = BuildFindFluent(_ => true, sort, projection);
            return findFluent.ToList();
        }

        public async Task<ICollection<T>> GetByPageAndQuantity(
            int page,
            int quantity,
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null)
        {
            var findFluent = BuildFindFluent(filter, sort, projection);
            return await findFluent.Limit(quantity).Skip((page - 1) * quantity).ToListAsync();
        }

        public ICollection<T> GetByPageAndQuantitySync(
            int page,
            int quantity,
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null)
        {
            var findFluent = BuildFindFluent(filter, sort, projection);
            return findFluent.Limit(quantity).Skip((page - 1) * quantity).ToList();
        }

        public async Task<T> GetSingleByExpression(
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null,
            int index = 1)
        {
            var findFluent = BuildFindFluent(filter, sort, projection);
            return await findFluent.Limit(1).Skip(index - 1).FirstOrDefaultAsync();
        }

        public T GetSingleByExpressionSync(
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null,
            int index = 1)
        {
            var findFluent = BuildFindFluent(filter, sort, projection);
            return findFluent.Limit(1).Skip(index - 1).FirstOrDefault();
        }

        public async Task<bool> Any(
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null,
            int index = 1)
        {
            var findFluent = BuildFindFluent(filter, sort, projection);
            return await findFluent.AnyAsync();
        }

        public bool AnySync(
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null,
            int index = 1)
        {
            var test = filter.Compile();
            var findFluent = BuildFindFluent(filter, sort, projection);
            return findFluent.Any();
        }

        public IEnumerable<T> QueryDb(Func<T, bool> filter)
        {
            var retult = MongoCollection.AsQueryable()
                .Where(filter);

            return retult.Distinct();
        }

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

        public T FirstOrDefaultSync(Expression<Func<T, bool>> where)
        {
            return MongoCollection.Find(where).FirstOrDefault();
        }

        public Task<T> FirstOrDefault(Expression<Func<T, bool>> where)
        {
            return MongoCollection.Find(where).FirstOrDefaultAsync();
        }

        public T SingleOrDefaultSync(Expression<Func<T, bool>> where)
        {
            return MongoCollection.Find(where).SingleOrDefault();
        }

        public Task<T> SingleOrDefault(Expression<Func<T, bool>> where)
        {
            return MongoCollection.Find(where).SingleOrDefaultAsync();
        }
    }
}