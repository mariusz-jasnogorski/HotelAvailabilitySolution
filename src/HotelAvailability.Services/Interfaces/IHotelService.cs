namespace HotelAvailability.Services.Interfaces
{
    public interface IHotelService
    {
        int GetAvailability(string hotelId, string roomType, DateTime startDate, DateTime? endDate = null);
        List<(DateTime Start, DateTime End, int Availability)> SearchAvailability(string hotelId, int daysAhead, string roomType);
    }
}
