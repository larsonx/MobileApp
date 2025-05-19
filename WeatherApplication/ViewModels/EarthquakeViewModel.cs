using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WeatherApplication.Services;

namespace WeatherApplication.ViewModels
{
    public class EarthquakeViewModel : INotifyPropertyChanged
    {
        private readonly EarthquakeAPI _earthquakeService;
        private List<EarthquakeData> earthquakeDataList;

        private int _currentPage = 0;
        private const int PageSize = 10;

        private bool _canGoNext;
        private bool _canGoPrevious;
        private int _totalPages;

        public List<EarthquakeData> EarthquakeDataList
        {
            get => earthquakeDataList;
            set
            {
                earthquakeDataList = value;
                OnPropertyChanged();
            }
        }

        public bool CanGoNext
        {
            get => _canGoNext;
            set
            {
                _canGoNext = value;
                OnPropertyChanged();
            }
        }

        public bool CanGoPrevious
        {
            get => _canGoPrevious;
            set
            {
                _canGoPrevious = value;
                OnPropertyChanged();
            }
        }

        public int CurrentPage => _currentPage + 1;

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
            }
        }

        public string PageDisplay => $"Page {CurrentPage} of {TotalPages}";

        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public EarthquakeViewModel()
        {
            _earthquakeService = new EarthquakeAPI();
            NextPageCommand = new Command(NextPage);
            PreviousPageCommand = new Command(PreviousPage);

            // Call this to initially load the data (First page)
            LoadPageData();
        }

        // Load data for the current page
        public async Task LoadPageData()
        {
            var totalEarthquakes = await _earthquakeService.GetEarthquakeDataCountAsync();
            _totalPages = (totalEarthquakes + PageSize - 1) / PageSize;

            var pageData = await _earthquakeService.GetEarthquakeDataForPageAsync(_currentPage, PageSize);
            EarthquakeDataList = pageData;

            CanGoPrevious = _currentPage > 0;
            CanGoNext = (_currentPage + 1) < _totalPages;
        }

        private void NextPage()
        {
            if (CanGoNext)
            {
                _currentPage++;
                LoadPageData().ConfigureAwait(false);
            }
        }

        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                _currentPage--;
                LoadPageData().ConfigureAwait(false);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}