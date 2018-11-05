using System;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace translate_spa.Models.ResponseModels
{
    public class ResponseModel : IResponseModel
    {
        public int StatusCode { get; set; }

        public string Status
        {
            get { return Enumerable.Range(200, 299).Contains(StatusCode) ? "OK" : "Error"; }
        }

        public string Response { get; set; }

        public bool LoadModal
        {
            get { return this.RedirectUrl?.IndexOf("modal", StringComparison.InvariantCultureIgnoreCase) == 0; }
        }

        public string ResponseText { get; set; }
        public bool Redirect { get; set; }
        public string RedirectUrl { get; set; }

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

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}