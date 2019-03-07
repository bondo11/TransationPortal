using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Serilog;
using translate_spa.Models;
using translate_spa.Models.Interfaces;
using translate_spa.Repositories;

namespace translate_spa.Querys
{
    public class GetEnvironment
    {
        readonly string _env;
        public bool hasEnvironment { get; private set; }
        public TranslationsEnvironment? Value { get; private set; }

        public GetEnvironment(string env, bool mandatory)
        {
            _env = env;
            this.hasEnvironment = Enum.TryParse<TranslationsEnvironment>(env, out var translationsEnvironment);
            if (!this.hasEnvironment)
            {
                if (mandatory)
                {
                    var enumValues = Enum.GetValues(typeof(TranslationsEnvironment));
                    throw new Exception($"Envionment missing, following are currently allowed: '{string.Join(", ", enumValues)}'");
                }

                Log.Debug($"Envionment missing'");
            }
            this.Value = this.hasEnvironment ? translationsEnvironment : (TranslationsEnvironment?)null;
        }
    }
}