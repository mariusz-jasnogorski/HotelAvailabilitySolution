//using Serilog;
using Microsoft.Extensions.Logging;

using HotelAvailability.Core.Interfaces;
using HotelAvailability.Services.Interfaces;

namespace HotelAvailability.Services.Implementations
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<HotelService> _logger;

        public HotelService(IHotelRepository hotelRepository, IBookingRepository bookingRepository, ILogger<HotelService> logger)
        {
            _hotelRepository = hotelRepository;
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        public int GetAvailability(string hotelId, string roomType, DateTime startDate, DateTime? endDate = null)
        {
            ValidateHotelAndRoomType(hotelId, roomType);
            
            if ((endDate != null) && (startDate > endDate))
            {
                throw new ArgumentException("Start date cannot be later than end date.");
            }
            
            _logger.LogInformation($"Checking availability for hotel {hotelId}, room type {roomType}, from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}.");

            var hotels = _hotelRepository.GetAll();
            var bookings = _bookingRepository.GetAll();

            var hotel = hotels.FirstOrDefault(h => h.Id == hotelId);
            var totalRooms = hotel.Rooms.Count(r => r.RoomType == roomType);
            var end = endDate ?? startDate;

            var bookedRooms = bookings
                .Where(b => b.HotelId == hotelId && b.RoomType == roomType &&
                            b.Arrival <= end && b.Departure >= startDate)
                .Count();

            var availability = totalRooms - bookedRooms;

            _logger.LogInformation($"Availability for hotel {hotelId}, room type {roomType}: {availability}");

            return availability;
        }

        public List<(DateTime Start, DateTime End, int Availability)> SearchAvailability(string hotelId, int daysAhead, string roomType)
        {
            ValidateHotelAndRoomType(hotelId, roomType);

            if (daysAhead <= 0)
            {
                throw new ArgumentException("Days ahead must be a positive number.");
            }

            _logger.LogInformation($"Searching availability for hotel {hotelId}, room type {roomType}, for the next {daysAhead} days.");

            var result = new List<(DateTime Start, DateTime End, int Availability)>();
            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(daysAhead);

            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {
                int availability = GetAvailability(hotelId, roomType, date);
                var rangeEnd = date.AddDays(1);

                if ((result.Count == 0) || (result.Last().Availability != availability))
                {
                    result.Add((date, rangeEnd, availability));
                }
                else
                {
                    result[result.Count - 1] = (result.Last().Start, rangeEnd, availability);
                }
            }

            if (result.Count == 0)
            {
                _logger.LogWarning($"No availability found for hotel {hotelId}, room type {roomType}, in the next {daysAhead} days.");
            }
            else
            {
                _logger.LogInformation($"Availability search for hotel {hotelId}, room type {roomType}, completed with results.");
            }

            return result;
        }

        private void ValidateHotelAndRoomType(string hotelId, string roomType)
        {
            var hotels = _hotelRepository.GetAll();

            if ((hotels?.Count == 0) || !hotels.Any(h => h.Id == hotelId))
            {
                throw new ArgumentException($"Hotel ID '{hotelId}' is invalid.");
            }

            var hotel = hotels.First(h => h.Id == hotelId);
           
            if (!hotel.Rooms.Any(rt => rt.RoomType == roomType))
            {
                throw new ArgumentException($"Room type '{roomType}' is invalid for hotel '{hotelId}'.");
            }
        }
    }
}
