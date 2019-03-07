using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Serilog;
using translate_spa.Models;
using translate_spa.Repositories;

namespace translate_spa.Actions
{
    public class GetFromQuery
    {
        readonly MongoRepository<Translation> _mongoRepository;

        public GetFromQuery(MongoRepository<Translation> baseRepository)
        {
            _mongoRepository = baseRepository;
        }
        public IEnumerable<Translation> Execute(Expression<Func<Translation, bool>> predicate)
        {
            var result = _mongoRepository.Query(predicate).ToList();
            Log.Debug($"returned '{result.Count()}' from db query.");
            return result;
        }
    }
}