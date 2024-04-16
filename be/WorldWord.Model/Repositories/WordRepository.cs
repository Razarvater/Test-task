using MongoDB.Driver;
using WorldWord.Context.Interfaces;
using WorldWord.Context.Models;

namespace WorldWord.Context.Repositories
{
    public class WordRepository : IWordRepository<Word>
    {
        private readonly IMongoCollection<Word> _collection;
        public WordRepository(WorldContext ctx) => _collection = ctx.Words;

        public async Task<List<Word>> GetListAsync(FilterDefinition<Word> filter) => await _collection.Find(filter).ToListAsync();

        public async Task AddAsync(Word item) => await _collection.InsertOneAsync(item);


        public IMongoCollection<Word> Query() => _collection;
    }
}
