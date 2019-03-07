using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using MongoDB.Driver;
using Serilog;
using translate_spa.Models;
using translate_spa.Models.Interfaces;
using translate_spa.MongoDB;
using translate_spa.Repositories;
using translate_spa.Utilities;

namespace translate_spa.Actions
{
    public class AddOrReplaceOne
    {
        readonly MongoRepository<Translation> _mongoRepository;
        private Expression<Func<Translation, bool>> _predicate;

        public AddOrReplaceOne(MongoRepository<Translation> baseRepository)
        {
            _mongoRepository = baseRepository;
            _predicate = PredicateBuilder.True<Translation>();
        }

        public void Execute(Translation translation)
        {
            SetPredicate(translation);
            new PrepareForInsert(translation).Execute();

            Log.Debug($"updating translation: {translation.ToString()}");
            //_mongoRepository.Update(_predicate, translation);
            var existing = _mongoRepository.SingleOrDefault(_predicate);
            if (existing == null)
            {
                Log.Debug($"Adding translation: {translation.ToString()}");
                new PrepareForInsert(translation).Execute();
                _mongoRepository.Add(translation);
                return;
            }
            new PrepareForInsert(translation).Execute();
            Log.Debug($"updating translation: {translation.ToString()}");
            Update(existing as Translation, translation);
            _mongoRepository.Update(translation);
        }

        private void Update(Translation oldTranslation, Translation newTranslation)
        {
            if (!oldTranslation.Key.Equals(newTranslation.Key, StringComparison.CurrentCulture))
            {
                oldTranslation.Key = newTranslation.Key;
                new PrepareForInsert(oldTranslation).Execute();
            }
            var values = (Language[])Enum.GetValues(typeof(Language));
            foreach (var language in values)
            {
                oldTranslation.SetValueByLanguage(language, newTranslation.GetByLanguage(language));
            }
        }

        private void SetPredicate(Translation translation)
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