using System;
using System.Collections.Generic;

using MongoDB.Bson.Serialization.Attributes;

namespace translate_spa.Models.Interfaces
{
    [BsonSerializer]
    public interface IToken
    {
        [BsonId]
        [BsonElement("id")]
        string Id { get; set; }

        [BsonElement("guid")]
        Guid TokenGuid { get; set; }

        [BsonElement("created")]
        DateTime Created { get; set; }

        [BsonElement("expire")]
        DateTime Expire { get; set; }

        [BsonId]
        string UserId { get; set; }

        void Extend(int days);
        bool IsValid();
    }
}