using System.Diagnostics;
using System.Text.Json;

namespace WeatherApplication.Services
{
    public class EarthquakeAPI
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;

        public EarthquakeAPI()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<int> GetEarthquakeDataCountAsync()
        {
            Uri uri = new Uri(string.Format(Constants.RestUrl));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var earthquakeApiResponse = JsonSerializer.Deserialize<EarthquakeApiResponse>(content, _serializerOptions);
                    return earthquakeApiResponse?.Features.Count ?? 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\tERROR {ex.Message}");
            }

            return 0;
        }

        public async Task<List<EarthquakeData>> GetEarthquakeDataForPageAsync(int pageIndex, int pageSize)
        {
            List<EarthquakeData> earthquakeDataList = new List<EarthquakeData>();
            Uri uri = new Uri(string.Format(Constants.RestUrl));

            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var earthquakeApiResponse = JsonSerializer.Deserialize<EarthquakeApiResponse>(content, _serializerOptions);
                    if (earthquakeApiResponse != null)
                    {
                        var pageData = earthquakeApiResponse.Features
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .ToList();

                        foreach (var feature in pageData)
                        {
                            var properties = feature.Properties;
                            earthquakeDataList.Add(new EarthquakeData
                            {
                                Magnitude = properties.Mag,
                                Location = properties.Place,
                                Date = DateTimeOffset.FromUnixTimeMilliseconds(properties.Time).DateTime,
                                Time = DateTimeOffset.FromUnixTimeMilliseconds(properties.Time).DateTime
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\tERROR {ex.Message}");
            }

            return earthquakeDataList;
        }

        public static class Constants
        {
            public static string RestUrl = "https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_day.geojson";
        }
    }


    public class EarthquakeApiResponse
    {
        public List<Feature> Features { get; set; } = new List<Feature>();
    }

    public class Feature
    {
        public Properties Properties { get; set; } = new Properties();
    }

    public class Properties
    {
        public double Mag { get; set; }
        public string Place { get; set; } = string.Empty;
        public long Time { get; set; } // Unix timestamp in milliseconds
    }

    public class EarthquakeData
    {
        public double Magnitude { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
    }
}
