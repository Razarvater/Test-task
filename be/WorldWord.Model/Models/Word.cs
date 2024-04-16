using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WorldWord.Context.Models
{
    public class Word
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public DateOnly CreateDate { get; set; }
        public required string Email { get; set; }
        public required string Value { get; set; }
        public required string Region { get; set; }
    }
}