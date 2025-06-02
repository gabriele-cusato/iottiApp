using IottiMobileApp.ViewModels;

namespace IottiMobileApp.Views
{
    public partial class MainPage : ContentPage
    {
        // Costruttore “vero” in DI
        private readonly MainViewModel _viewModel;

        public MainPage()
        : this(App.Services.GetRequiredService<MainViewModel>())
        {
        }

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Chiamata al toast quando la pagina appare
            await _viewModel.OnPageLoadedAsync();
        }

        private void OnInnerScrollViewScrolled(object sender, ScrolledEventArgs e)
        {

        }

        private void OnInnerScrollViewUnfocused(object sender, FocusEventArgs e)
        {

        }

        private void OnOpenCameraClicked(object sender, EventArgs e)
        {
            DisplayAlert("Titolo", "ciao", "OK");
        }

        private void OnCheckCloudClicked2(object sender, EventArgs e)
        {

        }

        private void OnCheckCloudClicked(object sender, EventArgs e)
        {

        }
    }

}
