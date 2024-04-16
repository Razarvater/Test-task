using System.Globalization;
using WorldWord.DTO;

namespace WorldWord.Api.Converters
{
    public static class RegionConverter
    {
        public static RegionDTO ToDTO(this RegionInfo region) =>
             new RegionDTO() { EnglishName = region.EnglishName, Name = region.Name };
    }
}
