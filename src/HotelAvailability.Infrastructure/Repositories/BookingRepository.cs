using HotelAvailability.Core.Interfaces;
using HotelAvailability.Core.Models;

namespace HotelAvailability.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(string filePath) : base(filePath)
        {
        }
    }
}
