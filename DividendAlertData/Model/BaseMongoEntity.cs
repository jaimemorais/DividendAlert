using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DividendAlertData.Model
{
    public class BaseMongoEntity
    {
        [BsonId]
        public Guid Id { get; set; }

    }
}
