using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WeatherApplication.Services;



namespace WeatherApplication.ViewModels
{
    public class ForecastViewModel : INotifyPropertyChanged
    {
        private string city;
        private ForecastData forecastData;
        private readonly ForecastAPI _forecastService;



        public string City
        {
            get => city;
            set
            {
                city = value;
                OnPropertyChanged();
            }
        }

        public ForecastData ForecastData
        {
            get => forecastData;
            set
            {
                forecastData = value;
                OnPropertyChanged();
            }
        }
        public ICommand FetchWeatherCommand { get; }

        public ForecastViewModel()
        {
            _forecastService = new ForecastAPI();
            FetchWeatherCommand = new Command(async () => await FetchForecast());
        }

        public async Task FetchForecast()
        {
            ForecastData = await _forecastService.GetForecastDataAsync(City);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}