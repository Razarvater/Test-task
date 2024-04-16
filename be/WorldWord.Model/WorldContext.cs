using MongoDB.Bson;
using MongoDB.Driver;
using WorldWord.Config;
using WorldWord.Context.Models;

namespace WorldWord.Context
{
    public class WorldContext
    {
        private readonly IMongoClient _client;
        private readonly  IMongoDatabase _database;
        public WorldContext(WorldWordConfiguration config)
        {
            DatabaseConfig cfg = config.MongoConfig;
            _client = new MongoClient(cfg.ConnectionString);

            _database = _client.GetDatabase(cfg.DatabaseName);
            InitIndexes();
        }

        public IMongoCollection<Word> Words =>
            _database.GetCollection<Word>("Word");

        public void InitIndexes()
        {
            string indexName = "dateIndex";
            string indexUniqueName = "uniqueEmailIndex";
            var indexNames = this.Words.Indexes.List().ToList<BsonDocument>()
                .SelectMany(i => i.Elements)
                .Where(e => string.Equals(e.Name, "name", StringComparison.CurrentCultureIgnoreCase))
                .Select(n => n.Value.ToString()).ToList();

            if (!indexNames.Contains(indexName))
            {
                var indexKeysDefinition = Builders<Word>.IndexKeys.Ascending(x => x.CreateDate).Ascending(x => x.Value);
                this.Words.Indexes.CreateOne(new CreateIndexModel<Word>(indexKeysDefinition, new CreateIndexOptions { Name = indexName }));
            }

            if (!indexNames.Contains(indexUniqueName))
            {
                var indexKeysDefinition = Builders<Word>.IndexKeys.Ascending(x => x.CreateDate).Ascending(x => x.Email);
                this.Words.Indexes.CreateOne(new CreateIndexModel<Word>(indexKeysDefinition, new CreateIndexOptions { Name = indexUniqueName, Unique = true }));
            }
        }
    }
}
