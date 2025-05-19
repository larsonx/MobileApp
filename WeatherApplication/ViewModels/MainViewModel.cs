using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WeatherApplication.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string city;

        public string City
        {
            get => city;
            set
            {
                if (city != value)
                {
                    city = value;
                    OnPropertyChanged();
                    WeatherVM.City = city;
                    ForecastVM.City = city;
                }
            }
        }

        public WeatherViewModel WeatherVM { get; }
        public ForecastViewModel ForecastVM { get; }

        public ICommand FetchCommand { get; }

        public MainViewModel()
        {
            WeatherVM = new WeatherViewModel();
            ForecastVM = new ForecastViewModel();

            FetchCommand = new Command(async () =>
            {
                await WeatherVM.FetchWeather();
                await ForecastVM.FetchForecast();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
