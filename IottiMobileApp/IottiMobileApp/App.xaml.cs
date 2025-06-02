using DbMobileModel.Models.IntermediateDb;
using DbMobileModel.Services.Interfaces;
using IottiMobileApp.Classes;
using IottiMobileApp.Views;
namespace IottiMobileApp
{
    public partial class App : Application
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public static IServiceProvider Services { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        /// <summary>
        /// Costruttore della classe App, è il secondo entry point dopo aver configurato mauiProgram, qui verrà caricata la pagina principale del dispositvo
        /// </summary>
        public App(IServiceProvider serviceProvider)
        {
            //questo è usato per lanciare mainPage con il viewModel e come prima pagina,
            //senza questo non si poteva usare il viewModel per la prima pagina caricata
            Services = serviceProvider;

            _ = CaricaUtenteAsync(); // Avvio asincrono

            InitializeComponent();
            ResourcesHelper.StartInitialization();
        }

        private async Task CaricaUtenteAsync()
        {
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);
            if (!isLoggedIn)
                return;

            try
            {
                var username = Preferences.Get("Username", "");

                var dbService = Services.GetRequiredService<IIntermediateDbService>();
                var utente = await dbService.GetUtenteByUsernameAsync(username);

                if (utente != null)
                    UserSession.UtenteCorrente = utente;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nel caricamento utente da AppShell: {ex}");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);
            var toastService = Services.GetRequiredService<IToastService>();

            if (isLoggedIn)
            {
                // Utente già loggato - crea AppShell con CustomWindow
                var appShell = new AppShell();
                return new Window(appShell);
            }
            else
            {
                // Utente non loggato - usa LoginPage con CustomWindow
                var loginPage = Services.GetRequiredService<LoginPage>();
                return new Window(new NavigationPage(loginPage));
            }
        }
    }
}