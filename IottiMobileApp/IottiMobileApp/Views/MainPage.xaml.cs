using IottiMobileApp.ViewModels;

namespace IottiMobileApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        : this(App.Services.GetRequiredService<MainViewModel>())
        {
        }

        // 2) Costruttore “vero” in DI
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
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
