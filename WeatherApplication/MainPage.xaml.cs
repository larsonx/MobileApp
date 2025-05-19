using WeatherApplication.ViewModels; // Add this using directive

namespace WeatherApplication
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
            // BindingContext = new WeatherViewModel();

        }
    }
}
