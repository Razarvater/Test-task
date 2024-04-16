using MongoDB.Driver;
using System.Globalization;
using WorldWord.Config;
using WorldWord.Context;
using WorldWord.Context.Models;
using WorldWord.Context.Repositories;
using WorldWord.DTO;

namespace WorldWord.Api.Services.Tests
{
    [TestClass()]
    public class WordServiceTests
    {
        private readonly WordRepository _wordRepository;
        public WordServiceTests()
        {
            //Enter connection string to test server
            WorldWordConfiguration configuration = new WorldWordConfiguration(string.Empty, new DatabaseConfig("mongodb://admin:123456@localhost", "test"));

            WorldContext ctx = new WorldContext(configuration);
            _wordRepository = new WordRepository(ctx);
            ctx.Words.DeleteMany(Builders<Word>.Filter.Where(x => true));
        }

        [TestMethod()]
        public async Task AddNewWordTest()
        {
            AddWordDTO[] words = new AddWordDTO[10];
            for (int i = 0; i < words.Length; i++)
            {
                string value = string.Empty;
                if (i >= 0 && i < 4)
                    value = "filter";
                else if (i >= 4 && i < 7)
                    value = "fitter";

                words[i] = new AddWordDTO
                {
                    Email = $"test{i}@mail.com",
                    Region = RegionInfo.CurrentRegion,
                    Value = value
                };
            }
            words[7].Value = "island";
            words[8].Value = "keyboard";
            words[9].Value = "desk";


            WordService service = new WordService(_wordRepository);
            for (int i = 0; i < 10; i++)
            {
                AddWordResponseDTO? response = await service.AddNewWordAsync(words[i]);
                Assert.IsNotNull(response);

                if (i >= 0 && i < 4)
                {
                    Assert.IsTrue(response.MyRegionStats.MostPopularWord.Value == words[i].Value && response.MyRegionStats.MostPopularWord.Count == i + 1);
                }
                else
                {
                    Assert.IsTrue(response.MyRegionStats.YourWord.Value == words[i].Value && response.MyRegionStats.YourWord.Count == (i >= 7 ? 1 : i - 3));
                    Assert.IsTrue(response.MyRegionStats.ClosestWords.Any(x => x.Value == words[i].Value));
                }
            }

            WordGroupDTO dto = await service.GetMostPopularWordAsync();
            Assert.IsTrue(dto.Value == "filter" && dto.Count == 4);
        }
    }
}