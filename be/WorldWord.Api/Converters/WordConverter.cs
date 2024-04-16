using MongoDB.Driver;
using WorldWord.Context.Models;
using WorldWord.DTO;

namespace WorldWord.Api.Converters
{
    public static class WordConverter
    {
        /// <summary>
        /// Converts <see cref="Word"/> to <see cref="WordGroupDTO"/>
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static IAggregateFluent<WordGroupDTO> ToGroupDTO(this IAggregateFluent<Word> that) =>
            that.Group(x => x.Value, x => new WordGroupDTO() { Value = x.Key, Count = x.Count() });
    }
}
