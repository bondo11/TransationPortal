using System.Collections.Generic;

using MongoDB.Bson.Serialization.Attributes;

namespace translate_spa.Models.Interfaces
{
    [BsonSerializer]
    public interface IUser
    {
        [BsonId]
        [BsonElement("id")]
        string Id { get; set; }

        [BsonElement("username")]
        string Username { get; set; }

        [BsonElement("email")]
        string Email { get; set; }

        [BsonElement("phone")]
        string Phone { get; set; }

        [BsonElement("password")]
        string Password { get; set; }

        void SetNewId();
    }
}