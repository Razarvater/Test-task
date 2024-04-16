using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WorldWord.DTO
{
    public class AddWordDTO
    {
        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression("^([A-Z]|[a-z])*$")]
        public string Value { get; set; }
        public RegionInfo? Region { get; set; }
    }

    public class AddWordResponseDTO
    {
        public RegionDTO Region { get; set; }
        public WordStatistic MyRegionStats { get; set; }
        public WordStatistic AllRegionStats { get; set; }
    }

    public class WordStatistic
    {
        public WordGroupDTO MostPopularWord { get; set; }
        public WordGroupDTO YourWord { get; set; }
        public List<WordGroupDTO> ClosestWords { get; set; }
    }

    public class WordGroupDTO
    {
        public string Value { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
