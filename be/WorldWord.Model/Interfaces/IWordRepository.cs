using MongoDB.Driver;
using WorldWord.Context.Models;

namespace WorldWord.Context.Interfaces
{
    public interface IWordRepository<T> where T : class
    {
        Task<List<T>> GetListAsync(FilterDefinition<Word> filter);
        Task AddAsync(T item);

        public IMongoCollection<T> Query();
    }
}
