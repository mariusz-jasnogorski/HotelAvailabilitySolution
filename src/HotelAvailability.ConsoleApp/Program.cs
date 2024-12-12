using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

using HotelAvailability.Core.Interfaces;
using HotelAvailability.Infrastructure.Repositories;
using HotelAvailability.Services.Implementations;
using HotelAvailability.Services.Interfaces; 

namespace HotelAvailability.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            try
            {
                Log.Information("Starting application...");

                var serviceProvider = ConfigureServices(args);

                var hotelService = serviceProvider.GetService<IHotelService>();

                while (true)
                {
                    Console.WriteLine("Enter command:");
                    var command = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(command)) break;

                    if (command.StartsWith("Availability"))
                    {
                        var parts = command.Replace("Availability(", "").Replace(")", "").Split(",", StringSplitOptions.TrimEntries);
                        var hotelId = parts[0];
                        var dateRange = parts[1];
                        var roomType = parts[2];

                        DateTime startDate, endDate;

                        if (dateRange.Contains('-'))
                        {
                            var dateParts = dateRange.Split('-');
                            startDate = DateTime.ParseExact(dateParts[0], "yyyyMMdd", null);
                            endDate = DateTime.ParseExact(dateParts[1], "yyyyMMdd", null);
                        }
                        else
                        {
                            startDate = DateTime.ParseExact(dateRange, "yyyyMMdd", null);
                            endDate = startDate;
                        }

                        var availability = hotelService.GetAvailability(hotelId, roomType, startDate, endDate);
                        Console.WriteLine($"Availability: {availability}");
                    }
                    else if (command.StartsWith("Search"))
                    {
                        var parts = command.Replace("Search(", "").Replace(")", "").Split(",", StringSplitOptions.TrimEntries);
                        var hotelId = parts[0];
                        var daysAhead = int.Parse(parts[1]);
                        var roomType = parts[2];

                        var availability = hotelService.SearchAvailability(hotelId, daysAhead, roomType);
                        var result = string.Join(", ", availability.Select(a => $"({a.Start:yyyyMMdd}-{a.End:yyyyMMdd}, {a.Availability})"));
                        Console.WriteLine(string.IsNullOrWhiteSpace(result) ? "" : result);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while running the application.");
            }
            finally
            {
                Log.Information("Shutting down application...");
                Log.CloseAndFlush();
            }
        }

        private static ServiceProvider ConfigureServices(string[] args)
        {
            // File paths from arguments
            var hotelFile = args[1];
            var bookingFile = args[3];

            // DI container setup
            var services = new ServiceCollection();

            // Register logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog();
            });

            // Register repositories
            services.AddSingleton<IHotelRepository>(sp => new HotelRepository(hotelFile));
            services.AddSingleton<IBookingRepository>(sp => new BookingRepository(bookingFile));

            // Register services
            services.AddSingleton<IHotelService, HotelService>();

            return services.BuildServiceProvider();
        }
    }
}
