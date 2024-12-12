using HotelAvailability.Infrastructure.Repositories;

namespace HotelAvailabilityTests
{
    [TestClass]
    public class BookingRepositoryTests
    {
        private string _testFilePath = Path.Combine(Path.GetTempPath(), "bookings.json");

        [TestInitialize]
        public void Setup()
        {
            var jsonData = @"
    [
        {
            ""hotelId"": ""H1"",
            ""arrival"": ""20240901"",
            ""departure"": ""20240903"",
            ""roomType"": ""SGL"",
            ""roomRate"": ""Standard""
        }
    ]";
            File.WriteAllText(_testFilePath, jsonData);
        }

        [TestMethod]
        public void GetAllBookings_ShouldReturnBookings()
        {
            var repository = new BookingRepository(_testFilePath);
            var bookings = repository.GetAll();

            Assert.AreEqual(1, bookings.Count);
            Assert.AreEqual("H1", bookings[0].HotelId);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
    }
}