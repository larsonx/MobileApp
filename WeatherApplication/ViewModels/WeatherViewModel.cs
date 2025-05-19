using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WeatherApplication.Services;


namespace WeatherApplication.ViewModels
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        private string city;
        private WeatherData weatherData;
        private readonly WeatherAPI _weatherService;


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

        public ICommand FetchWeatherCommand { get; }

        public WeatherViewModel()
        {
            _weatherService = new WeatherAPI();
            FetchWeatherCommand = new Command(async () => await FetchWeather());
        }

        public async Task FetchWeather()
        {
            WeatherData = await _weatherService.GetWeatherDataAsync(City);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
