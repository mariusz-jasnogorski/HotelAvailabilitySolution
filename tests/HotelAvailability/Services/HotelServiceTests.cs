using Microsoft.Extensions.Logging;
using Moq;

using HotelAvailability.Core.Interfaces;
using HotelAvailability.Core.Models;
using HotelAvailability.Services.Implementations;

namespace HotelAvailabilityTests
{
    [TestClass]
    public class HotelServiceTests
    {
        private Mock<IHotelRepository> _hotelRepositoryMock;
        private Mock<IBookingRepository> _bookingRepositoryMock;
        private HotelService _hotelService;

        [TestInitialize]
        public void Setup()
        {
            _hotelRepositoryMock = new Mock<IHotelRepository>();
            _bookingRepositoryMock = new Mock<IBookingRepository>();

            _hotelRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Hotel>
        {
            new Hotel
            {
                Id = "H1",
                Name = "Test Hotel",
                Rooms = new List<Room>
                {
                    new Room { RoomType = "SGL", RoomId = "101" },
                    new Room { RoomType = "SGL", RoomId = "102" },
                    new Room { RoomType = "DBL", RoomId = "201" }
                }
            }
        });

            _bookingRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<Booking>
        {
            new Booking
            {
                HotelId = "H1",
                RoomType = "SGL",
                Arrival = DateTime.Today,
                Departure = DateTime.Today.AddDays(2)
            }
        });

            _hotelService = new HotelService(_hotelRepositoryMock.Object, _bookingRepositoryMock.Object, Mock.Of<ILogger<HotelService>>());
        }

        [TestMethod]
        public void GetAvailability_ShouldReturnCorrectAvailability()
        {
            var availability = _hotelService.GetAvailability("H1", "SGL", DateTime.Today, DateTime.Today.AddDays(1));

            Assert.AreEqual(1, availability);
        }

        [TestMethod]
        public void GetAvailability_NoHotel_ShouldThrowArgumentException()
        {
            string invalidHotelId = "H2";

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentException>(() =>
                _hotelService.GetAvailability(invalidHotelId, "SGL", DateTime.Today, DateTime.Today.AddDays(1)));

            Assert.AreEqual($"Hotel ID '{invalidHotelId}' is invalid.", exception.Message);
        }

        [TestMethod]
        public void SearchAvailability_ShouldReturnCorrectResults()
        {
            var results = _hotelService.SearchAvailability("H1", 4, "SGL");

            Assert.AreEqual(2, results.Count);
        }
    }
}