using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WorldWord.DTO
{
    public class AddWordDTO
    {
        [EmailAddress]
        public required string Email { get; set; }

        [RegularExpression("^([A-Z]|[a-z])*$")]
        public required string Value { get; set; }
        public RegionInfo? Region { get; set; }
    }

    public class AddWordResponseDTO
    {
        public required RegionDTO Region { get; set; }
        public required WordStatistic MyRegionStats { get; set; }
        public required WordStatistic AllRegionStats { get; set; }
    }

    public class WordStatistic
    {
        public required WordGroupDTO MostPopularWord { get; set; }
        public required WordGroupDTO YourWord { get; set; }
        public required List<WordGroupDTO> ClosestWords { get; set; }
    }

    public class WordGroupDTO
    {
        public required string Value { get; set; }
        public required int Count { get; set; }
    }
}
