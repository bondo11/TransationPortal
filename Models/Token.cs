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
    public class Token : IToken
    {
        [BsonId]
        public string Id { get; set; }
        public Guid TokenGuid { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expire { get; set; }
        public string UserId { get; set; }

        public void Extend(int days = 30)
        {
            var date = DateTime.Now;
            date.AddDays(days);
            this.Expire = date;
        }

        public bool IsValid()
        {
            var now = DateTime.Now;
            return Expire > now;
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