using DbMobileModel.Models.IntermediateDb;
using DbMobileModel.Services.Interfaces;
using IottiMobileApp.Classes;
using IottiMobileApp.Views;
namespace IottiMobileApp
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

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
            //dentro le preferences si possono salvare anche oggetti serializzati in json, non so se sia proprio una pratica usata ma si puo fare
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

            if (isLoggedIn)
            {
                return new Window(new AppShell());
                //appShell non ha un servizio perchè non utilizza la DI
            }
            else
            {
                var login = Services.GetRequiredService<LoginPage>();
                return new Window(new NavigationPage(login));
            }

        }
    }
}