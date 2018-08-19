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
    public class UpdateTranslation
    {
        readonly MongoRepository<Translation> _mongoRepository;
        readonly ILogger _log;
        private Expression<Func<Translation, bool>> _predicate;

        public UpdateTranslation(MongoRepository<Translation> baseRepository, ILogger log)
        {
            _mongoRepository = baseRepository;
            _log = log;
            _predicate = PredicateBuilder.True<Translation>();
        }

        public void Execute(Translation translation)
        {
            SetPredicate(translation);
            var existing = _mongoRepository.Single(_predicate);
            ExistingGuard(existing);
            _log.Debug($"UpdateTranslation: updating {translation.ToString()}");
            _mongoRepository.Update(translation);
        }

        public async Task<Task> ExecuteAsync(Translation translation)
        {
            SetPredicate(translation);
            var existing = _mongoRepository.Single(_predicate);
            ExistingGuard(existing);
            _log.Debug($"UpdateTranslation: updating {translation.ToString()}");
            return _mongoRepository.UpdateAsync(translation);
        }

        void ExistingGuard(ITranslation existing)
        {
            if (existing == null)
            {
                throw new Exception("Cannot find the desired translation, you have to create it");
            }

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