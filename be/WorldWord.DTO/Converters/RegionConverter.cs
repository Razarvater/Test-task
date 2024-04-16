using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace WorldWord.DTO.Converters
{
    public class RegionConverter : JsonConverter<RegionInfo>
    {
        public override RegionInfo? ReadJson(JsonReader reader, Type objectType, RegionInfo? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                JObject obj = JObject.Load(reader);
                string? name = obj["name"].Value<string>();
                if (string.IsNullOrEmpty(name))
                    return null;

                return new RegionInfo(name);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, RegionInfo? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
