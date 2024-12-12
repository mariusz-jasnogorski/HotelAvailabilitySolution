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
            var hotelsArgIndex = Array.IndexOf(args, "--hotels");
            var bookingsArgIndex = Array.IndexOf(args, "--bookings");

            if (hotelsArgIndex < 0 || bookingsArgIndex < 0 ||
                hotelsArgIndex == args.Length - 1 || bookingsArgIndex == args.Length - 1)
            {
                Console.WriteLine("Please provide --hotels <file> and --bookings <file>");
                return;
            }

            var hotelsPath = args[hotelsArgIndex + 1];
            var bookingsPath = args[bookingsArgIndex + 1];

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Warning()
                .CreateLogger();

            Log.Information("Starting application...");

            var serviceProvider = ConfigureServices(hotelsPath, bookingsPath);

            var hotelService = serviceProvider.GetService<IHotelService>();

            while (true)
            {
                Console.WriteLine("Enter command (blank line to exit):");
                var command = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(command)) break;

                switch (GetCommandType(command))
                {
                    case "Availability":
                        HandleAvailability(hotelService, command);
                        break;

                    case "Search":
                        HandleSearch(hotelService, command);
                        break;

                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
            
            Log.Information("Shutting down application...");
            Log.CloseAndFlush();
        }

        static string GetCommandType(string command)
        {
            if (command.StartsWith("Availability", StringComparison.OrdinalIgnoreCase)) return "Availability";
            if (command.StartsWith("Search", StringComparison.OrdinalIgnoreCase)) return "Search";
            return "Unknown";
        }

        static void HandleAvailability(IHotelService hotelService, string command)
        {
            try
            {
                // Extract parameters
                var parts = ExtractParameters(command, "Availability");
                var hotelId = parts[0];
                var dateRange = parts[1];
                var roomType = parts[2];

                var (startDate, endDate) = ParseDateRange(dateRange);

                var availability = hotelService.GetAvailability(hotelId, roomType, startDate, endDate);

                Console.WriteLine($"Availability: {availability}");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        static void HandleSearch(IHotelService hotelService, string command)
        {
            try
            { 
                // Extract parameters
                var parts = ExtractParameters(command, "Search");
                var hotelId = parts[0];
                var daysAhead = int.Parse(parts[1]);
                var roomType = parts[2];

                var availability = hotelService.SearchAvailability(hotelId, daysAhead, roomType);
                var result = string.Join(", ", availability.Select(a => $"({a.Start:yyyyMMdd}-{a.End:yyyyMMdd}, {a.Availability})"));

                Console.WriteLine(string.IsNullOrWhiteSpace(result) ? "" : result);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        static (DateTime start, DateTime end) ParseDateRange(string range)
        {
            if (range.Contains('-'))
            {
                var dateParts = range.Split('-');
                return (ParseExact(dateParts[0]), ParseExact(dateParts[1]));
            }

            var singleDate = ParseExact(range);
            return (singleDate, singleDate);

            static DateTime ParseExact(string dateStr) => DateTime.ParseExact(dateStr, "yyyyMMdd", null);
        }

        static string[] ExtractParameters(string command, string prefix)
        {
            return command
                .Replace(prefix + "(", "")
                .Replace(")", "")
                .Split(",", StringSplitOptions.TrimEntries);
        }

        private static ServiceProvider ConfigureServices(string hotelsFile, string bookingsFile)
        {           
            // DI container setup
            var services = new ServiceCollection();

            // Register logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog();
            });

            // Register repositories
            services.AddSingleton<IHotelRepository>(sp => new HotelRepository(hotelsFile));
            services.AddSingleton<IBookingRepository>(sp => new BookingRepository(bookingsFile));

            // Register services
            services.AddSingleton<IHotelService, HotelService>();

            return services.BuildServiceProvider();
        }
    }
}
