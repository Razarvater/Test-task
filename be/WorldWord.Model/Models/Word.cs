using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WorldWord.Context.Models
{
    public class Word
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public DateOnly CreateDate { get; set; }
        public string Email { get; set; }
        public string Value { get; set; }
        public string Region { get; set; }
    }
}