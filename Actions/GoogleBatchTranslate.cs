using Google.Apis.Services;
using Google.Apis.Translate.v2;
using Google.Apis.Translate.v2.Data;
using TranslationsResource = Google.Apis.Translate.v2.Data.TranslationsResource;

using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using translate_spa.Models;
using translate_spa.Utilities;

namespace translate_spa.Actions
{
    public class GoogleBatchTranslate
    {
        private readonly List<Translation> _translations;
        private readonly TranslateService _translateService;
        private readonly static string _googleApiKey = Startup.Configuration.GetSection ("GoogleTranslateApi") ["ApiKey"];
        private readonly static string _googleApplicationName = Startup.Configuration.GetSection ("GoogleTranslateApi") ["ApplicationName"];

        public GoogleBatchTranslate (List<Translation> translations)
        {
            _translations = translations;
            _translateService = new TranslateService (new BaseClientService.Initializer
            {
                ApplicationName = _googleApplicationName,
                    ApiKey = _googleApiKey,
            });
        }

        public List<Translation> Execute ()
        {
            //var languages = Enum.GetValues(typeof(Language));
            var languages = Enum.GetValues (typeof (Language))
                .Cast<Language> ()
                .Where (x => x != Language.Da);

            foreach (var lang in languages)
            {
                var values = _translations.Select (x => x.GetByLanguage (lang));
                var translatedValues = GetTranslations (values, lang);
                int index = 0;
                foreach (var translatedText in translatedValues)
                {
                    _translations[index].SetValueByLanguage (lang, translatedText);
                    index++;
                }
            }

            return _translations;
        }

        IEnumerable<string> GetTranslations (IEnumerable<string> values, Language distLang)
        {
            //var response = _translateService.Translations.List(_translation.Da, distLang).Execute();
            var response = _translateService.Translations.Translate (new TranslateTextRequest ()
            {
                Format = "html",
                    Source = Language.Da.ToString (),
                    Target = distLang.ToString (),
                    Q = values.ToArray ()
            }).Execute ();

            var translations = response.Translations;

            Log.Debug ($"translated '{values.Count()}' from '{Language.Da.ToString()}' to '{distLang.ToString()}'");

            return translations.Select (x => x.TranslatedText);
        }
    }
}