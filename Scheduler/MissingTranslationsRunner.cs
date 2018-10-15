using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using translate_spa.Models;
using translate_spa.MongoDB.DbBuilder;
using translate_spa.Repositories;
using translate_spa.Tasks;

namespace translate_spa.Scheduler
{
    public class MissingTranslationsRunner : HostedService
    {
        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());

            var translations = mongoRepository.All()
                .Where(x => string.IsNullOrEmpty(x.Branch) &&
                    x.HasMissingTranslation()).ToList();

            _log.Debug($"Running missing translations task. Missing translations: {translations.Count()}");
            await new MissingTranslationsTask(translations, _log).ExecuteAsync();
        }
    }
}