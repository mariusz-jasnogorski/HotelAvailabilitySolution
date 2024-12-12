using HotelAvailability.Core.Interfaces;
using HotelAvailability.Core.Models;

namespace HotelAvailability.Infrastructure.Repositories
{
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {
        public HotelRepository(string filePath) : base(filePath)
        {
        }
    }
}
