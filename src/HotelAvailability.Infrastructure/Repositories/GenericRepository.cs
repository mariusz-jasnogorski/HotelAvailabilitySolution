using Newtonsoft.Json;
using HotelAvailability.Core.Interfaces;
using Newtonsoft.Json.Converters;

public class GenericRepository<T> : IGenericRepository<T>
{
    private readonly string _filePath;

    public GenericRepository(string filePath)
    {
        _filePath = filePath;
    }

    public List<T> GetAll()
    {
        var json = File.ReadAllText(_filePath);

        var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyyMMdd" };

        return JsonConvert.DeserializeObject<List<T>>(json, dateTimeConverter);
    }
}