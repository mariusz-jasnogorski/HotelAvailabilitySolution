using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace HotelAvailability.Infrastructure.Utilities
{
    public static class FileDataReader
    {
        public static List<T> ReadJsonFile<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);

            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyyMMdd" };

            return JsonConvert.DeserializeObject<List<T>>(json, dateTimeConverter);
        }
    }

}
