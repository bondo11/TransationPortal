using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Serilog;
using translate_spa.Models;
using translate_spa.Models.Interfaces;
using translate_spa.MongoDB;
using translate_spa.Repositories;

namespace translate_spa.Actions
{
    public class DeleteTranslation
    {
        readonly MongoRepository<Translation> _mongoRepository;
        private Expression<Func<Translation, bool>> _predicate;

        public DeleteTranslation(MongoRepository<Translation> baseRepository)
        {
            _mongoRepository = baseRepository;
            _predicate = PredicateBuilder.True<Translation>();
        }

        public void Execute(Translation translation)
        {
            SetPredicate(translation);
            Log.Debug($"Deleting translation: {translation.ToString()}");
            _mongoRepository.Delete(_predicate);
        }

        public void Execute(string id)
        {
            SetPredicate(id);
            Log.Debug($"Deleting translation by Id: {id}");
            _mongoRepository.Delete(_predicate);
        }

        public async Task<Task> ExecuteAsync(Translation translation)
        {
            SetPredicate(translation);
            Log.Debug($"Deleting translation async: {translation.ToString()}");
            return _mongoRepository.DeleteAsync(_predicate);
        }

        public async Task<Task> ExecuteAsync(string id)
        {
            SetPredicate(id);
            Log.Debug($"Deleting translation async by Id: {id}");
            return _mongoRepository.DeleteAsync(_predicate);
        }

        void InsureTranslationsCanBeInserted(Translation translation)
        {
            var existing = _mongoRepository.Any(_predicate);
            if (existing)
            {
                throw new Exception("A similar translation already exist. Delete that first.");
            }
            translation.SetNewId();
        }

        private void SetPredicate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new Exception("Id cannot be empty on delete");
            };

            _predicate = _predicate.And(x => x.Id == id);
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