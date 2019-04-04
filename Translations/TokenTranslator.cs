using System;
using System.Collections;
using System.Linq;
using translate_spa.Models;

namespace translate_spa.Translations
{
    public class TokenTranslation
    {
        public Guid GetGuid (string tokenHash)
        {
            var base64Converter = new Base64Convert ();
            var tokenHashString = base64Converter.Decode (tokenHash);
            var tokenGuid = GetToken (tokenHashString);

            return new Guid ();
        }

        Guid GetToken (string tokenString)
        {
            var validTokenGuid = Guid.TryParse (tokenString, out Guid tokenGuid);

            if (!validTokenGuid)
            {
                throw new UnauthorizedAccessException ("Invalid token");
            }

            return tokenGuid;
        }
    }
}