namespace WeatherApplication;
using WeatherApplication.ViewModels; // Add this using directive

public partial class AardbevingenPage : ContentPage
{
    private EarthquakeViewModel _viewModel;

    public AardbevingenPage()
    {
        InitializeComponent();
        _viewModel = new EarthquakeViewModel();
        BindingContext = _viewModel;
    }

    // No need to load data when the page first appears anymore
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Optionally load the first page of data when the page appears
        // await _viewModel.LoadPageData(); // This is optional if you want to load the first page right away.
    }
}
