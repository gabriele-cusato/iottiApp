using System.ComponentModel;
using IottiMobileApp.Classes;
using IottiMobileApp.Views;
using IottiMobileApp.FontCodes;
using DbMobileModel.Services.Interfaces;

namespace IottiMobileApp
{
    public partial class AppShell : Shell
    {
        //guida DI: https://youtu.be/xx1mve2AQr4?si=vLJ7evygBO9IJAdW

        public string userImage { get; set; }

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(FieraPage), typeof(FieraPage));

            if (UserSession.UtenteCorrente != null)
            {
                var nome = UserSession.UtenteCorrente.UtnUsername;
                userImage = string.IsNullOrWhiteSpace(nome) ? "?" : nome[0].ToString().ToUpper();
            }
            else
            {
                userImage = FASolid.User;
            }

            BindingContext = this;
        }

        //protected override async void OnAppearing()
        //{
        //    //ora che ho caricato la shell e le routes faccio la redirezione se necessario
        //    base.OnAppearing();

        //    // 1) Controlla il flag di login
        //    bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

        //    // 2) Se non loggato, vai alla pagina di login
        //    if (!isLoggedIn)
        //    {
        //        //non si puo usare la root assoluta con // quando c'è una sola pagina, ritorna un eccezione se si mette //
        //        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        //    }
        //}

        public event PropertyChangedEventHandler? UserPropertyChanged;
        protected void OnUserPropertyChanged1(string name) =>
            UserPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public async void OnNavigateToMainPage(object sender, EventArgs e)
        {

            await Shell.Current.GoToAsync("///" + nameof(MainPage));
            Shell.Current.FlyoutIsPresented = false;
        }

        public async void OnNavigateToLoginPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
            Shell.Current.FlyoutIsPresented = false;
        }

        public async void OnNavigateToRegisterPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
            Shell.Current.FlyoutIsPresented = false;
        }

        public async void OnNavigateToFieraPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(FieraPage));
            Shell.Current.FlyoutIsPresented = false;
        }

        public async void onLogout(object sender, EventArgs e)
        {
            Preferences.Set("IsLoggedIn", false);
            Preferences.Set("Username", "");

            UserSession.UtenteCorrente = null;

            await Shell.Current.GoToAsync(nameof(LoginPage));
            Shell.Current.FlyoutIsPresented = false;
        }
    }
}
