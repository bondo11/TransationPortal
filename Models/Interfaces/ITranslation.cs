using System.Collections.Generic;

using MongoDB.Bson.Serialization.Attributes;

namespace translate_spa.Models.Interfaces
{
    [BsonSerializer]
    public interface ITranslation
    {
        [BsonId]
        [BsonElement("id")]
        string Id { get; set; }

        [BsonElement("key")]
        string Key { get; set; }

        [BsonElement("da")]
        string Da { get; set; }

        [BsonElement("en")]
        string En { get; set; }

        [BsonElement("sv")]
        string Sv { get; set; }

        [BsonElement("nb")]
        string Nb { get; set; }

        [BsonElement("branch")]
        string Branch { get; set; }

        [BsonElement("environment")]
        TranslationsEnvironment? Environment { get; set; }

        Dictionary<string, string> GetProperties();

        void SetNewId();
    }
}