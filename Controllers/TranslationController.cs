using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using translate_spa.Controllers.ActionFilters;
using translate_spa.Models;
using translate_spa.MongoDB.DbBuilder;
using translate_spa.Repositories;

namespace translate_spa.Controllers
{

    public class TranslationController : Controller
    {

        private readonly ILogger<TranslationController> _log;

        public TranslationController(ILogger<TranslationController> log)
        {
            _log = log;
        }

        [HttpGet]
        public async Task<IEnumerable<Translation>> Index()
        {
            _log.LogDebug(null, null, "Getting all translations");
            var baseRepository = new BaseRepository<Translation>(new BaseDbBuilder());
            var translations = baseRepository.GetAll();
            return await translations;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Translation))]
        [ProducesResponseType(404)]
        public async Task<Translation> Add([FromBody] Translation translation)
        {
            if (string.IsNullOrWhiteSpace(translation.Key))
            {
                throw new Exception($"translation cannot have an empty key. Please set a valid key.");
            }
            var baseRepository = new BaseRepository<Translation>(new BaseDbBuilder());
            var existing = await baseRepository.Any(x => x.Key == translation.Key);
            if (existing)
            {
                throw new Exception($"Adding dublicate translation-key not allowed. the used translationkey are allready in use");
            }

            baseRepository.AddSync(translation);

            var result = baseRepository.GetSingleByExpressionSync(x => x.Key == translation.Key);

            StatusCode(StatusCodes.Status201Created);

            return result;
        }

        [HttpPatch]
        [ProducesResponseType(200, Type = typeof(Translation))]
        [ProducesResponseType(404)]
        public async Task Update([FromBody] Translation translation)
        {
            if (string.IsNullOrWhiteSpace(translation.Id))
            {
                throw new Exception($"no id has been given.");
            }
            if (string.IsNullOrWhiteSpace(translation.Key))
            {
                throw new Exception($"translation cannot have an empty key. Please set a valid key.");
            }

            var baseRepository = new BaseRepository<Translation>(new BaseDbBuilder());

            await baseRepository.Update(item: translation, key: translation.Id);

            StatusCode(StatusCodes.Status202Accepted);
        }

        [HttpDelete("~/api/[controller]/[action]/{id}")]
        public async Task Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new Exception($"no id has been given.");
            }

            var baseRepository = new BaseRepository<Translation>(new BaseDbBuilder());

            var existing = await baseRepository.GetSingleByExpression(x => x.Id == id);

            if (existing == null)
            {
                throw new Exception($"Could not find a record with id of '{id}'.");
            }

            await baseRepository.Delete(x => x.Id == id);

            StatusCode(StatusCodes.Status202Accepted);
        }

        [HttpGet("~/api/[controller]/[action]/{id}")]
        public async Task<IEnumerable<Translation>> Query(string id)
        {
            var baseRepository = new BaseRepository<Translation>(new BaseDbBuilder());
            var result = baseRepository.GetAllSync();
            var queryResult = result.Where(x => x.Key.StartsWith(id + ".", System.StringComparison.CurrentCultureIgnoreCase)).ToList();

            queryResult.AddRange(result.Where(x => CommenKey(x.Key)));

            return queryResult;
        }

        [HttpGet("~/api/[controller]/[action]/{env}/{id}")]
        public async Task<JsonResult> AngularQuery(Language id, string env)
        {
            var baseRepository = new BaseRepository<Translation>(new BaseDbBuilder());
            var result = baseRepository.GetAllSync();
            var queryResult = result.Where(x => x.Key.StartsWith(env + ".", System.StringComparison.CurrentCultureIgnoreCase)).ToList();

            var resultDictionary = queryResult.ToDictionary(t => t.Key, t => t.GetByLanguage(id));

            var jsonResult = JsonHelper.Unflatten(resultDictionary);

            return new JsonResult(jsonResult);
        }

        bool CommenKey(string key)
        {
            var envs = Startup.Configuration.GetSection("Dictionary")["Environments"].Split(',');

            return !envs.Any(x => key.StartsWith(x + ".", System.StringComparison.CurrentCultureIgnoreCase));
        }

        [HttpPost("~/api/[controller]/[action]")]
        public async Task updateFromJson([FromBody] List<Translation> translations)
        {
            var baseRepository = new BaseRepository<Translation>(new BaseDbBuilder());
            //var taskList = translations.Select(x => UpdateTranslation(baseRepository, x));
            var taskList = new List<Task>();
            foreach (var item in translations)
            {
                var exist = baseRepository.AnySync(x => x.Key == item.Key && x.Branch == item.Branch);
                if (exist)
                {
                    taskList.Add(UpdateTranslation(baseRepository, item));
                    continue;
                }
                taskList.Add(AddTranslation(baseRepository, item));
            }

            foreach (var task in taskList)
            {
                await task;
            }

            StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("~/api/[controller]/[action]")]
        public async Task updateFromOldJson([FromBody] List<OldTranslation> translations)
        {
            var translated = translations.Select(x => new Translation()
            {
                Key = x.KEY,
                    Da = x.DA,
                    En = x.EN,
                    Sv = x.SV,
                    Nb = x.NB,
            });

            var baseRepository = new BaseRepository<Translation>(new BaseDbBuilder());
            //var taskList = translated.Select(x => UpdateTranslation(baseRepository, x));

            var taskList = new List<Task>();
            foreach (var item in translated)
            {
                var existing = baseRepository.SingleOrDefaultSync(x => x.Key == item.Key && x.Branch == item.Branch);
                if (existing != null)
                {
                    item.Id = existing.Id;
                    taskList.Add(baseRepository.ReplaceOne(existing, item));
                    continue;
                }
                taskList.Add(AddTranslation(baseRepository, item));
            }

            foreach (var task in taskList)
            {
                await task;
            }

            StatusCode(StatusCodes.Status201Created);
        }

        async Task<Task> AddTranslation(BaseRepository<Translation> baseRepo, Translation translation)
        {
            return baseRepo.Add(translation);
        }

        async Task<Task> UpdateTranslation(BaseRepository<Translation> baseRepo, Translation translation)
        {
            return baseRepo.Update(x => x.Key == translation.Key && x.Branch == translation.Branch, translation);
        }
    }
}