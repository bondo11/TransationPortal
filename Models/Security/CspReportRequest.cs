using System.Text;

using Newtonsoft.Json;

namespace translate_spa.Models.Security
{
    public class CspReportRequest
    {
        [JsonProperty(PropertyName = "csp-report")]
        public CspReport CspReport { get; set; }

        override public string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(this.GetType());
            stringBuilder.Append("\n");
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                stringBuilder.Append($"\t{propertyInfo.Name}: {propertyInfo.GetValue(this, null)}\n");
            }

            return stringBuilder.ToString();
        }
    }

    public class CspReport
    {
        [JsonProperty(PropertyName = "document-uri")]
        public string DocumentUri { get; set; }

        [JsonProperty(PropertyName = "referrer")]
        public string Referrer { get; set; }

        [JsonProperty(PropertyName = "violated-directive")]
        public string ViolatedDirective { get; set; }

        [JsonProperty(PropertyName = "effective-directive")]
        public string EffectiveDirective { get; set; }

        [JsonProperty(PropertyName = "original-policy")]
        public string OriginalPolicy { get; set; }

        [JsonProperty(PropertyName = "blocked-uri")]
        public string BlockedUri { get; set; }

        [JsonProperty(PropertyName = "status-code")]
        public int StatusCode { get; set; }

        override public string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(this.GetType());
            stringBuilder.Append("\n");
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                stringBuilder.Append($"\t{propertyInfo.Name}: {propertyInfo.GetValue(this, null)}\n");
            }

            return stringBuilder.ToString();
        }
    }
}