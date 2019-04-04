using System;

namespace translate_spa.Translations
{
    public class Base64Convert
    {
        public string Decode(string base64String)
        {
            var data = Convert.FromBase64String(base64String);
            var base64Decoded = System.Text.ASCIIEncoding.ASCII.GetString(data);
            return base64Decoded;
        }

        public string Encode(string sourceString)
        {
            var data = System.Text.Encoding.UTF8.GetBytes(sourceString);
            return Encode(data);
        }

        public string Encode(byte[] data)
        {
            var base64String = Convert.ToBase64String(data);
            return base64String;
        }
    }
}

