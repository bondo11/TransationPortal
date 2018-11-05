using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using NLog;

using translate_spa.Models;
using translate_spa.Models.Interfaces;
using translate_spa.Repositories;

namespace translate_spa.Actions
{
    public class GetFromQuery
    {
        readonly MongoRepository<Translation> _mongoRepository;
        readonly ILogger _log;

        public GetFromQuery(MongoRepository<Translation> baseRepository, ILogger log)
        {
            _mongoRepository = baseRepository;
            _log = log;
        }
        public IEnumerable<Translation> Execute(Expression<Func<Translation, bool>> predicate)
        {
            var result = _mongoRepository.Query(predicate).ToList();
            _log.Debug($"returned '{result.Count()}' from db query.");
            return result;
        }
    }
}