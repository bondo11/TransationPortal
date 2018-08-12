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
    public class Translation : IEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Key { get; set; }

        public string Da { get; set; }

        public string En { get; set; }

        public string Sv { get; set; }

        public string Nb { get; set; }

        public string Branch { get; set; }

        public string GetByLanguage(Language lang)
        {
            var fieldName = lang.ToString();
            var type = this.GetType();
            var property = type.GetProperty(fieldName);
            var value = (string)property.GetValue(this, null);
            return value;
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
    }
}