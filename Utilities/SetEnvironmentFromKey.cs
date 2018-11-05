using System;

using translate_spa.Models;

namespace translate_spa.Utilities
{
    public class SetEnvironmentFromKey
    {
        readonly Translation _translation;

        public SetEnvironmentFromKey(Translation translation)
        {
            _translation = translation;
        }

        public void Execute()
        {
            if (_translation.Key.StartsWith("Desktop.App.", StringComparison.CurrentCultureIgnoreCase))
            {
                _translation.Environment = TranslationsEnvironment.Desktop;
                return;
            }
            if (_translation.Key.StartsWith("Sign.", StringComparison.CurrentCultureIgnoreCase))
            {
                _translation.Environment = TranslationsEnvironment.Sign;
                return;
            }
            if (_translation.Key.StartsWith("Portal.", StringComparison.CurrentCultureIgnoreCase))
            {
                _translation.Environment = TranslationsEnvironment.Portal;
                return;
            }
            if (_translation.Key.StartsWith("Web.", StringComparison.CurrentCultureIgnoreCase))
            {
                _translation.Environment = TranslationsEnvironment.Web;
                return;
            }
            if (_translation.Key.StartsWith("Api.", StringComparison.CurrentCultureIgnoreCase))
            {
                _translation.Environment = TranslationsEnvironment.Api;
                return;
            }
            if (_translation.Key.StartsWith("Desktop.", StringComparison.CurrentCultureIgnoreCase))
            {
                _translation.Environment = TranslationsEnvironment.OldDesktop;
                return;;
            }
            _translation.Environment = TranslationsEnvironment.Common;
        }
    }
}