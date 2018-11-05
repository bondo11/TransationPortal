using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using translate_spa.Actions;
using translate_spa.Controllers.ActionFilters;
using translate_spa.Models;
using translate_spa.MongoDB;
using translate_spa.MongoDB.DbBuilder;
using translate_spa.Querys;
using translate_spa.Repositories;
using translate_spa.Tasks;
using translate_spa.Utilities;

namespace translate_spa.Controllers
{
    public class TranslationController : BaseController
    {
        [HttpGet]
        public async Task<IEnumerable<Translation>> Index()
        {
            _log.Debug("Getting all translations");
            var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());
            var predicate = PredicateBuilder.True<Translation>();
            predicate = new SetBranchPedicate(predicate, _log).Execute(_Branch);

            var result = new GetFromQuery(mongoRepository, _log).Execute(predicate);
            return result;
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

            var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());

            new SetEnvironmentFromKey(translation).Execute();
            return new AddTranslation(mongoRepository, _log).Execute(translation)as Translation;
        }

        [HttpPost("~/api/[controller]/[action]")]
        [ProducesResponseType(200, Type = typeof(Translation))]
        [ProducesResponseType(404)]
        public async Task<Translation> GoogleTranslateMissing([FromBody] Translation translation)
        {
            return new GoogleTranslate(translation, _log).Execute();
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

            var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());
            var existing = mongoRepository.SingleOrDefault(x => x.Key == translation.Key && x.Branch == translation.Branch);
            if (existing != null && existing.Id != translation.Id)
            {
                throw new Exception($"Adding dublicate translation-key not allowed. the used translationkey are allready in use");
            }

            new SetEnvironmentFromKey(translation).Execute();

            new UpdateTranslation(mongoRepository, _log).Execute(translation);

            StatusCode(StatusCodes.Status202Accepted);
        }

        [HttpDelete("~/api/[controller]/[action]/{id}")]
        public async Task Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new Exception($"no id has been given.");
            }

            var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());

            _log.Debug($"Delete: Deleting '{id}'");

            mongoRepository.Delete(id);

            StatusCode(StatusCodes.Status202Accepted);
        }

        [HttpGet("~/api/[controller]/[action]/{env}")]
        public async Task<IEnumerable<Translation>> Query(TranslationsEnvironment env)
        {
            var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());
            var queryExpression = PredicateBuilder.True<Translation>();

            queryExpression = queryExpression.And(x => x.Environment == env);

            if (_Branch.HasBranch)
            {
                _log.Debug($"Query: adding Branch predicate for branch: '{_Branch.Value}'");

                queryExpression = queryExpression.And(x => x.Branch == null);
            }

            if (env != TranslationsEnvironment.Desktop && env != TranslationsEnvironment.OldDesktop)
            {
                queryExpression = queryExpression.Or(x => x.Environment == TranslationsEnvironment.Common);
            }

            var queryResult = mongoRepository.Query(queryExpression).ToList();

            _log.Debug($"Query: returning '{queryResult.Count()}' environment '{env.ToString()}''");

            return queryResult;
        }

		[HttpGet("~/api/[controller]/[action]/{env}")]
		public async Task<IEnumerable<OldTranslation>> OldQuery(TranslationsEnvironment env)
		{
			var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());
			var queryExpression = PredicateBuilder.True<Translation>();

			queryExpression = queryExpression.And(x => x.Environment == env);

			if (_Branch.HasBranch)
			{
				_log.Debug($"Query: adding Branch predicate for branch: '{_Branch.Value}'");

				queryExpression = queryExpression.And(x => x.Branch == null);
			}

			if (env != TranslationsEnvironment.Desktop && env != TranslationsEnvironment.OldDesktop)
			{
				queryExpression = queryExpression.Or(x => x.Environment == TranslationsEnvironment.Common);
			}

			var queryResult = await mongoRepository.QueryAsync(queryExpression);

			_log.Debug($"Query: returning '{queryResult.Count()}' environment '{env.ToString()}''");

			var result = queryResult.Select(x => new OldTranslation()
			{
				KEY = x.Key,
				DA = x.Da,
				EN = x.En,
				SV = x.Sv,
				NB = x.Nb,
			});

			return result;
		}

		[HttpGet("~/api/[controller]/[action]/{env}/{lang}")]
        public async Task<JsonResult> AngularQuery(Language lang, string env)
        {
            var environment = new GetEnvironment(env, _log, true);
            var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());
            var predicate = PredicateBuilder.True<Translation>();

            if (_Branch.HasBranch)
            {
                predicate = predicate.And(x => x.Branch == _Branch.Value);
            }

            predicate = predicate.And(x => x.Environment == environment.Value);

            var result = mongoRepository.Query(predicate);
            _log.Debug($"AngularQuery: returning '{result.Count()}' entries for language'{lang}' on environment '{environment.Value}'");

			// TODO: maybe this should be changed, at least for the desktop, we skip "Desktop.", but maybe we should skip "Desktop.App." on the 
			// new desktop, and i dont know, what we should do on the web.
			// HENCE, maybe create a function for this, and may enable skip on 'dot' via appsettings.
            var resultDictionary = result.ToDictionary(t => string.Join(".", t.Key.Split(".").Skip(1)), t => t.GetByLanguage(lang));

            var jsonResult = JsonHelper.Unflatten(resultDictionary);

            return new JsonResult(jsonResult);
        }

        [HttpPost("~/api/[controller]/[action]")]
        public async Task updateFromJson([FromBody] List<Translation> translations)
        {
            var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());
            foreach (var item in translations)
            {
                _log.Debug($"updateFromJson : Key {item.Key} allready exist, updating");
                new AddOrReplaceOne(mongoRepository, _log).Execute(item);
                /* var exist = mongoRepository.Any(x => x.Key == item.Key && x.Branch == item.Branch);
                if (exist)
                {
                    _log.Debug($"updateFromJson : Key {item.Key} allready exist, updating");
                    new UpdateTranslation(mongoRepository, _log).Execute(item);
                    continue;
                }

                _log.Debug($"updateFromJson : adding {item.ToString()}");
                new AddTranslation(mongoRepository, _log).Execute(item); */
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
                    Branch = string.Empty
            });

            var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());

            var taskList = new List<Task>();
            foreach (var item in translated)
            {
                InsetTranslation(mongoRepository, item, _Branch.Value);
            }
            StatusCode(StatusCodes.Status201Created);
        }

        private void InsetTranslation(MongoRepository<Translation> mongoRepository, Translation item, string branch)
        {
            //new AddOrReplaceOne(mongoRepository, _log).Execute(item);
            new SetEnvironmentFromKey(item).Execute();
            item.SetNewId();
            var predicate = PredicateBuilder.True<Translation>();
            predicate = predicate.And(x => x.Environment == item.Environment);
            if (!string.IsNullOrWhiteSpace(branch))
            {
                predicate = predicate.And(x => x.Branch == branch);
            }
            predicate = predicate.And(x => x.Key == item.Key);

            var existing = mongoRepository.SingleOrDefault(x => x.Key == item.Key && x.Branch == branch);

            if (existing != null)
            {
                if (existing.Equals(item))
                {
                    _log.Debug($"the two objects are identical, continuing.");
                    return;
                }
                item.Id = existing.Id;
                _log.Debug($"updateFromOldJson : Key {item.Key} allready exist, updating");
                mongoRepository.Update(item);
                return;
            }
            _log.Debug($"updateFromOldJson : adding {item.ToString()}");
            mongoRepository.Add(item);
        }

        [HttpGet("~/api/[controller]/[action]")]
        public async Task ConvertAllItems()
        {
            var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());
            var predicate = PredicateBuilder.True<Translation>();
            predicate = predicate.And(x => x.Environment == null);
            predicate = predicate.And(x => x.Branch == null);

            var result = mongoRepository.Query(predicate);
            foreach (var item in result)
            {
                new SetEnvironmentFromKey(item).Execute();
                mongoRepository.Update(item);
            }
        }

        [HttpGet("~/api/[controller]/[action]")]
        public async Task Notify()
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