using HotelAvailability.Infrastructure.Repositories;

namespace HotelAvailabilityTests
{
    [TestClass]
    public class HotelRepositoryTests
    {
        private string _testFilePath = Path.Combine(Path.GetTempPath(), "hotels.json");

        [TestInitialize]
        public void Setup()
        {
            // Sample JSON data for testing
            var jsonData = @"
    [
        {
            ""id"": ""H1"",
            ""name"": ""Hotel Test"",
            ""roomTypes"": [
                { ""code"": ""SGL"", ""description"": ""Single Room"" }
            ],
            ""rooms"": [
                { ""roomType"": ""SGL"", ""roomId"": ""101"" }
            ]
        }
    ]";
            File.WriteAllText(_testFilePath, jsonData);
        }

        [TestMethod]
        public void GetAllHotels_ShouldReturnHotels()
        {
            var repository = new HotelRepository(_testFilePath);
            var hotels = repository.GetAll();

            Assert.AreEqual(1, hotels.Count);
            Assert.AreEqual("H1", hotels[0].Id);
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