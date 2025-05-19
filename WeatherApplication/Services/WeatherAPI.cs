using System.Diagnostics;
using System.Text.Json;

namespace WeatherApplication.Services
{
    public class WeatherAPI
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;

        public WeatherAPI()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<WeatherData> GetWeatherDataAsync(string city)
        {
            WeatherData weatherData = new WeatherData();
            Uri uri = new Uri(string.Format(Constants.RestUrl, city, Constants.ApiKey));

            try
            {
                Debug.WriteLine($"Requesting weather data for {city} from {uri}");
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Response content: {content}");
                    var weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(content, _serializerOptions);
                    if (weatherApiResponse != null)
                    {
                        weatherData.City = city;
                        weatherData.Temperature = weatherApiResponse.Main.Temp;
                        weatherData.Humidity = weatherApiResponse.Main.Humidity;
                        weatherData.WindSpeed = weatherApiResponse.Wind.Speed;
                        weatherData.Condition = weatherApiResponse.Weather[0].Main;
                        Debug.WriteLine($"Deserialized weather data: {weatherData}");
                    }
                }
                else
                {
                    Debug.WriteLine($"Response status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\tERROR {ex.Message}");
            }

            return weatherData;
        }
        public static class Constants
        {
            public static string ApiKey = "bfaa9142b2039f905711700a1dab06dc";

            // Ensure the API endpoint includes necessary parameters
            public static string RestUrl = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric";
        }

    }
}



public class WeatherApiResponse
{
    public List<WeatherCondition> Weather { get; set; } = new List<WeatherCondition>();
    public MainWeatherInfo Main { get; set; } = new MainWeatherInfo();
    public Wind Wind { get; set; } = new Wind();
}

public class WeatherData
{
    public string City { get; set; } = string.Empty;
    public float Temperature { get; set; }
    public string Condition { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public float WindSpeed { get; set; }
}

public class WeatherCondition
{
    public string Main { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class MainWeatherInfo
{
    public float Temp { get; set; }
    public int Humidity { get; set; }


}

public class Wind
{
    public float Speed { get; set; }
}

