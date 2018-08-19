using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using NLog;

using translate_spa.Models;
using translate_spa.Models.Interfaces;
using translate_spa.MongoDB;
using translate_spa.Repositories;

namespace translate_spa.Actions
{
    public class AddTranslation
    {
        readonly MongoRepository<Translation> _mongoRepository;
        readonly ILogger _log;
        private Expression<Func<Translation, bool>> _predicate;

        public AddTranslation(MongoRepository<Translation> baseRepository, ILogger log)
        {
            _mongoRepository = baseRepository;
            _log = log;
            _predicate = PredicateBuilder.True<Translation>();
        }

        public ITranslation Execute(Translation translation)
        {
            SetPredicate(translation);
            InsureTranslationsCanBeInserted(translation);
            _log.Debug($"Adding translation: {translation.ToString()}");
            _mongoRepository.Add(translation);
            return translation;
        }

        public async Task<Translation> ExecuteAsync(Translation translation)
        {
            SetPredicate(translation);
            InsureTranslationsCanBeInserted(translation);
            _log.Debug($"Adding translation async: {translation.ToString()}");
            await _mongoRepository.AddAsync(translation);

            return translation;
        }

        void InsureTranslationsCanBeInserted(Translation translation)
        {
            var existing = _mongoRepository.Any(_predicate);
            if (existing)
            {
                throw new Exception($"A similar translation already exist. Delete that one first. Translation: '{translation.ToString()}'");
            }
            translation.SetNewId();
        }

        private void SetPredicate(ITranslation translation)
        {
            if (!string.IsNullOrWhiteSpace(translation.Id))
            {
                _predicate = _predicate.And(x => x.Id == translation.Id);
                return;
            }
            if (translation.Key == null)
            {
                throw new ArgumentException("Key cannot be null");
            }
            if (translation.Environment != null)
            {
                _predicate = _predicate.And(x => x.Branch == translation.Branch);
            }
            _predicate = _predicate.And(x => x.Key == translation.Key);
        }
    }

}