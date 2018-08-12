using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using translate_spa.Models.Interfaces;
using translate_spa.Utilities;

namespace translate_spa.Models
{

    [Serializable]
    public class OldTranslation
    {
        public string KEY { get; set; }
        public string DA { get; set; }
        public string EN { get; set; }
        public string SV { get; set; }
        public string NB { get; set; }

        public string GetByLanguage(Language lang)
        {
            var fieldName = lang.ToString();
            var value = this.GetType().GetProperty(fieldName, BindingFlags.IgnoreCase).ToString();

            return value;
        }

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