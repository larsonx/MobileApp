
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using WeatherApplication.Services;
using static WeatherApplication.Services.ForecastAPI;

namespace WeatherApplication.ViewModels
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        private string city;
        private WeatherData weatherData;
        private readonly WeatherAPI _restService;
        private float maxTempToday;

        public WeatherViewModel()
        {
            _restService = new WeatherAPI();
            FetchWeatherCommand = new Command(async () => await OnFetchWeather());
        }
        public class ForecastAPI
        {
            // Existing code...

            public async Task<float> GetMaxTemperatureTodayAsync(string city)
            {
                var forecast = await GetHourlyForecastAsync(city);
                return forecast.Max(f => f.Temperature);
            }

            // Existing code...
        }

        private ForecastAPI _forecastService = new ForecastAPI();

        public async Task FetchMaxForecastAsync()
        {
            float maxToday = await _forecastService.GetMaxTemperatureTodayAsync(City);
            Debug.WriteLine($"Max temp today in {City}: {maxToday}°C");
        }

        public string City
        {
            get => city;
            set
            {
                city = value;
                OnPropertyChanged();
            }
        }

        public WeatherData WeatherData
        {
            get => weatherData;
            set
            {
                weatherData = value;
                OnPropertyChanged();
            }
        }

        private List<ForecastEntry> forecastList;
        public List<ForecastEntry> ForecastList
        {
            get => forecastList;
            set
            {
                forecastList = value;
                OnPropertyChanged();
            }
        }

        public float MaxTempToday
        {
            get => maxTempToday;
            set
            {
                maxTempToday = value;
                OnPropertyChanged();
            }
        }

        public ICommand FetchWeatherCommand { get; }

        private async Task OnFetchWeather()
        {
            // Fetch the current weather data
            WeatherData = await _restService.RefreshDataAsync(City);

            // Fetch the max temperature for today
            MaxTempToday = await _forecastService.GetMaxTemperatureTodayAsync(City);

            // Fetch the hourly forecast data and bind it to ForecastList
            ForecastList = await _forecastService.GetHourlyForecastAsync(City);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
