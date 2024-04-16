using MongoDB.Bson;
using MongoDB.Driver;
using WorldWord.Api.Converters;
using WorldWord.Context.Interfaces;
using WorldWord.Context.Models;
using WorldWord.DTO;

namespace WorldWord.Api.Services
{
    public class WordService
    {
        private readonly IWordRepository<Word> _wordRepository;

        public WordService(IWordRepository<Word> wordRepository)
        {
            _wordRepository = wordRepository;
        }

        /// <summary>
        /// Current <see cref="DateTime"/> static for scope
        /// </summary>
        private readonly DateTime _now = DateTime.UtcNow;

        /// <summary>
        /// Filter by date
        /// </summary>
        private FilterDefinition<Word> _filterByDate => Builders<Word>.Filter.Where(x => x.CreateDate == DateOnly.FromDateTime(_now.Date));

        private static FilterDefinition<Word> getfilterByRegion(string regionName) =>
            Builders<Word>.Filter.Where(x => x.Region == regionName);

        /// <summary>
        /// Generates a Levenshtein Filter in a <see cref="BsonDocument"/>.
        /// </summary>
        /// <param name="search">The search word.</param>
        /// <param name="maxDistance">The maximum distance allowed between the search word and the word in the specified column.</param>
        /// <param name="searchColumnName">The name of the column that contains the word to compare with the search word.</param>
        /// <returns></returns>
        private static BsonDocument getFilterByLevenstein(string search, int maxDistance = 1, string searchColumnName = nameof(Word.Value))
        {
            return new BsonDocument
                            {
                                {
                                    "$expr", new BsonDocument
                                    {
                                        {"$function", new BsonDocument
                                            {
                                                { "body", $@"
                                                    function levenshteinDistance(str1, str2) {{
                                                    let dp = new Array(str1.length + 1).fill(0).map(() => new Array(str2.length + 1).fill(0));

                                                    for (let i = 0; i <= str1.length; i++) {{
                                                        dp[i][0] = i;
                                                    }}

                                                    for (let j = 0; j <= str2.length; j++) {{
                                                        dp[0][j] = j;
                                                    }}

                                                    for (let i = 1; i <= str1.length; i++) {{
                                                        for (let j = 1; j <= str2.length; j++) {{
                                                            let cost = (str1[i - 1] === str2[j - 1]) ? 0 : 1;
                                                            dp[i][j] = Math.min(Math.min(dp[i - 1][j] + 1, dp[i][j - 1] + 1), dp[i - 1][j - 1] + cost);
                                                        }}
                                                    }}

                                                    return dp[str1.length][str2.length] <= {maxDistance};
                                                    }}"
                                                },
                                                { "args", new BsonArray(new List<string>() {search, string.Format("${0}", searchColumnName)})},
                                                { "lang", "js" }
                                            }
                                        }
                                    }
                            }};
        }


        /// <summary>
        /// Adds new word and returns info based on its popularity.
        /// </summary>
        /// <param name="dto">Word, Email and Region</param>
        /// <returns> Stats </returns>
        public async Task<AddWordResponseDTO?> AddNewWordAsync(AddWordDTO dto)
        {
            string RegionName = dto.Region!.Name;

            //Check if the word already has been added by email that day
            if (await buildBaseQuery().Match(x => x.Email == dto.Email).AnyAsync())
                return null;

            await _wordRepository.AddAsync(new Word() { Email = dto.Email, Value = dto.Value, CreateDate = DateOnly.FromDateTime(_now.Date), Region = RegionName });

            Task<WordGroupDTO> mostPopularInRegionTask = GetMostPopularWordAsync(RegionName);
            Task<WordGroupDTO> mostPopularInAllTask = GetMostPopularWordAsync();
            Task<List<WordGroupDTO>> closestWordsInRegionTask = getAllClosestWordsAsync(dto.Value, RegionName);
            Task<List<WordGroupDTO>> closestWordsInAllTask = getAllClosestWordsAsync(dto.Value);

            await Task.WhenAll(mostPopularInRegionTask, mostPopularInAllTask, closestWordsInRegionTask, closestWordsInAllTask);

            WordGroupDTO mostPopularInRegion = mostPopularInRegionTask.Result;
            WordGroupDTO mostPopularInAll = mostPopularInAllTask.Result;
            List<WordGroupDTO> closestWordsInRegion = closestWordsInRegionTask.Result;
            List<WordGroupDTO> closestWordsInAll = closestWordsInAllTask.Result;

            WordStatistic myRegionStats = new WordStatistic()
            {
                MostPopularWord = mostPopularInRegion,
                ClosestWords = closestWordsInRegion,
                YourWord = new WordGroupDTO() { Value = dto.Value, Count = closestWordsInRegion.FirstOrDefault(x => x.Value == dto.Value)?.Count ?? 1 }
            };

            WordStatistic allRegionStats = new WordStatistic()
            {
                MostPopularWord = mostPopularInAll,
                ClosestWords = closestWordsInAll,
                YourWord = new WordGroupDTO() { Value = dto.Value, Count = closestWordsInAll.FirstOrDefault(x => x.Value == dto.Value)?.Count ?? 1 }
            };

            return new AddWordResponseDTO() { Region = dto.Region.ToDTO(), MyRegionStats = myRegionStats, AllRegionStats = allRegionStats };
        }


        /// <summary>
        /// Gets the list of closest words
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="regionName"></param>
        /// <returns></returns>
        private async Task<List<WordGroupDTO>> getAllClosestWordsAsync(string searchPattern, string regionName = "")
        {
            /*
            var regexPattern = new StringBuilder($"^((.?{searchPattern})|({searchPattern}.?)");
            for (int i = 0; i < searchPattern.Length; i++)
                regexPattern.Append($"({searchPattern.Remove(i, 1).Insert(i, ".?")}){(i < searchPattern.Length - 1 ? "|" : string.Empty)}");
            regexPattern.Append(")$");

            var filter = Builders<Word>.Filter.Regex(x => x.Value, new BsonRegularExpression(regexPattern.ToString(), "i"));
            */
            IAggregateFluent<Word> query;
            if (string.IsNullOrEmpty(regionName))
            {
                query = buildBaseQuery();
            }
            else
            {
                query = buildBaseQueryWithRegion(regionName);
            }

            int maxLevensteinDistance = 1;
            return await query.Match(x => Math.Abs(x.Value.Length - searchPattern.Length) <= maxLevensteinDistance)
                 .Match(getFilterByLevenstein(searchPattern, maxLevensteinDistance))
                 .ToGroupDTO()
                 .ToListAsync();
        }

        /// <summary>
        /// Gets the most popular word
        /// </summary>
        /// <returns></returns>
        public async Task<WordGroupDTO> GetMostPopularWordAsync(string regionName = "")
        {
            SortDefinition<WordGroupDTO> sortDefinition = Builders<WordGroupDTO>.Sort.Descending(x => x.Count).Ascending(x => x.Value);
            IAggregateFluent<Word> query;
            if (string.IsNullOrEmpty(regionName))
            {
                query = buildBaseQuery();
            }
            else
            {
                query = buildBaseQueryWithRegion(regionName);
            }

            /*
            var pipe = new EmptyPipelineDefinition<Word>().Match(getfilterByRegion(regionName)).Group(x => x.Value, x => new WordGroupDTO() { Value = x.Key, Count = x.Count() }).Sort(sortDefinition);
            var pipe2 = new EmptyPipelineDefinition<Word>().Group(x => x.Value, x => new WordGroupDTO() { Value = x.Key, Count = x.Count() }).Sort(sortDefinition);

            Not All indexes work
            var queryTest = _wordRepository.Query().Aggregate().Match(_filterByDate).Facet(AggregateFacet.Create("test", pipe), AggregateFacet.Create("test2", pipe2));

            foreach (var item in await queryTest.ToListAsync())
            {
                await Console.Out.WriteLineAsync(item.ToJson());
            }*/

            return await query
                 .ToGroupDTO()
                 .Sort(sortDefinition)
                 .FirstOrDefaultAsync();
        }

        private IAggregateFluent<Word> buildBaseQueryWithRegion(string regionName) =>
            buildBaseQuery().Match(getfilterByRegion(regionName));

        private IAggregateFluent<Word> buildBaseQuery() =>
            _wordRepository.Query().Aggregate().Match(_filterByDate);
    }
}