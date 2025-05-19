using System.Diagnostics;
using System.Text.Json;


namespace WeatherApplication.Services
{
    public class ForecastAPI
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;

        public ForecastAPI()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<ForecastData> GetForecastDataAsync(string city)
        {
            ForecastData forecastData = new ForecastData();
            Uri uri = new Uri(string.Format(Constants.ForecastUrl, city, Constants.ApiKey));

            Debug.WriteLine($"[DEBUG] Fetching forecast data for city: {city}");
            Debug.WriteLine($"[DEBUG] Request URL: {uri}");

            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                Debug.WriteLine($"[DEBUG] HTTP Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"[DEBUG] Raw JSON response:\n{content.Substring(0, Math.Min(500, content.Length))}"); // Print first 500 chars

                    var apiResponse = JsonSerializer.Deserialize<ForecastApiResponse>(content, _serializerOptions);

                    if (apiResponse != null)
                    {
                        forecastData.City = apiResponse.City.Name;
                        Debug.WriteLine($"[DEBUG] City from API: {forecastData.City}");
                        Debug.WriteLine($"[DEBUG] Total forecast items: {apiResponse.List.Count}");

                        var grouped = apiResponse.List
                            .GroupBy(f => DateTimeOffset.FromUnixTimeSeconds(f.Dt).Date)
                            .Take(7);

                        foreach (var group in grouped)
                        {
                            var dailyTemps = group.Select(g => g.Main.Temp).ToList();
                            forecastData.SevenDayForecast.Add(new DailyForecast
                            {
                                Date = group.Key,
                                MinTemperature = dailyTemps.Min(),
                                MaxTemperature = dailyTemps.Max()
                            });

                            Debug.WriteLine($"[DEBUG] {group.Key:yyyy-MM-dd} → Min: {dailyTemps.Min()}°C, Max: {dailyTemps.Max()}°C");

                            if (group.Key == DateTime.Now.Date)
                            {
                                forecastData.TodayHourly.AddRange(group.Select(h => new HourlyForecast
                                {
                                    Time = DateTimeOffset.FromUnixTimeSeconds(h.Dt).DateTime,
                                    Temperature = h.Main.Temp
                                }));

                                Debug.WriteLine($"[DEBUG] Added {forecastData.TodayHourly.Count} hourly entries for today.");
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("[ERROR] Failed to deserialize API response.");
                    }
                }
                else
                {
                    Debug.WriteLine($"[ERROR] Failed to get forecast: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EXCEPTION] Forecast error: {ex.Message}");
            }

            return forecastData;
        }
    }
    // JSON models for /forecast endpoint
    public class ForecastApiResponse
    {
        public CityInfo City { get; set; } = new();
        public List<ForecastItem> List { get; set; } = new();
    }

    public class CityInfo
    {
        public string Name { get; set; } = string.Empty;
    }

    public class ForecastItem
    {
        public long Dt { get; set; }
        public MainWeatherInfo Main { get; set; } = new();
    }

    public class ForecastData
    {
        public string City { get; set; } = string.Empty;
        public List<HourlyForecast> TodayHourly { get; set; } = new();
        public List<DailyForecast> SevenDayForecast { get; set; } = new();
    }

    public class HourlyForecast
    {
        public DateTime Time { get; set; }
        public float Temperature { get; set; }
    }

    public class DailyForecast
    {
        public DateTime Date { get; set; }
        public float MinTemperature { get; set; }
        public float MaxTemperature { get; set; }
    }

    public static class Constants
    {
        public static string ApiKey = "bfaa9142b2039f905711700a1dab06dc";
        public static string ForecastUrl = "https://api.openweathermap.org/data/2.5/forecast?q={0}&appid={1}&units=metric";
    }
}