namespace HotelAvailability.Core.Interfaces
{
    public interface IGenericRepository<T>
    {
        List<T> GetAll();
    }
}
