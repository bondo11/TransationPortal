using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    public class Translation : ITranslation, IEquatable<Translation>
    {
        [BsonId]

        public string Id { get; set; }

        [BsonElement("key")]
        public string Key { get; set; }

        [BsonElement("da")]
        public string Da { get; set; }

        [BsonElement("en")]
        public string En { get; set; }

        [BsonElement("sv")]
        public string Sv { get; set; }

        [BsonElement("nb")]
        public string Nb { get; set; }

        [BsonElement("branch")]
        [BsonIgnoreIfNull]
        public string Branch { get; set; } = string.Empty;

        public bool HasMissingTranslation()
        {
            var languages = Enum.GetValues(typeof(Language)).Cast<Language>();
            return languages.Any(x => string.IsNullOrEmpty(this.GetByLanguage(x)));
        }

        [BsonElement("environment")]
        [BsonIgnoreIfNull]
        public TranslationsEnvironment? Environment { get; set; }

        /*  public Translation()
         {
             new SetEnvironmentFromKey(this).Execute();
         } */
        public void SetNewId()
        {
            this.Id = ObjectId.GenerateNewId().ToString();
        }

        public string GetByLanguage(Language lang)
        {
            var fieldName = lang.ToString();
            var type = this.GetType();
            var property = type.GetProperty(fieldName);
            var value = (string)property.GetValue(this, null);
            return value;
        }

        public void SetValueByLanguage(Language lang, string value)
        {
            var fieldName = lang.ToString();
            var type = this.GetType();
            var property = type.GetProperty(fieldName);
            property.SetValue(this, value);
        }

        public Dictionary<string, string> GetProperties()
        {
            var properties = new Dictionary<string, string>();
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                if (propertyInfo.Name == "Id")
                {
                    continue;
                }
                properties.Add(propertyInfo.Name, (string)propertyInfo.GetValue(this, null));
            }
            return properties;
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

        public bool Equals(Translation other)
        {
            if (this.Id != other.Id)
            {
                return false;
            }

            if (!this.Key.Equals(other.Key, StringComparison.CurrentCulture))
            {
                return false;
            }

            var values = (Language[])Enum.GetValues(typeof(Language));
            foreach (var language in values)
            {
                if (!string.Equals(this.GetByLanguage(language), other.GetByLanguage(language)))
                {
                    return false;
                }
            }

            if (!string.Equals(this.Branch, other.Branch))
            {
                return false;
            }

            if (this.Environment != other.Environment)
            {
                return false;
            }

            return true;
        }
    }
}