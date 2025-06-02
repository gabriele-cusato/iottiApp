using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbMobileModel.DTO;
using DbMobileModel.Models.IntermediateDb;
using DbMobileModel.Services.Interfaces;
using IottiMobileApp.Classes;
using IottiMobileApp.Views;

namespace IottiMobileApp.ViewModels
{
    //partial viene usato perche community toolkit crea un'altra parte della classe in automatico per far funzionare i decoratori, con partial i 2 pezzi della classe possono essere uniti
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IRemoteAuthService _authService;

        [ObservableProperty]
        public string? username;
        [ObservableProperty]
        public string? password;
        [ObservableProperty]
        public bool isBusy;

        public LoginViewModel(IRemoteAuthService authService)
        {
            _authService = authService;
            IsBusy = false;
        }

        //public LoginViewModel()
        //{
        //}
        /*
            verrà prodotto in automatico del codice come questo:
            public string Username
            {
                get => username;
                set => SetProperty(ref username, value);
            }
            quindi poi quando si vanno ad utilizzare questi attributi bisogna indicarli con la prima lettera maiuscola
         */
        //questi attributi devono rimanere separati dai DTO:
        /*
         * i ViewModel gestiscono lo stato e la logica di presentazione, mentre i DTO servono solo a trasferire dati tra strati
         * sono POCO privi di logica di binding, e farli diventare "observable" li renderebbe contaminati da responsabilità di UI 
         */

        [RelayCommand]
        public async Task LoginAsync()
        {
            IsBusy = true;
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                IsBusy = false;
                await Shell.Current.DisplayAlert("Errore", "Valorizzare tutti i valori", "OK");
                return;
            }

            LoginDto loginDto = new LoginDto
            {
                username = Username!,
                password = Password!
            };

            Utente? user = await _authService.LoginAsync(loginDto);
            if (user != null)
            {
                UserSession.UtenteCorrente = user;
                Preferences.Set("IsLoggedIn", true);
                Preferences.Set("Username", user.UtnUsername);

                // Crea AppShell con ToastService
                var appShell = new AppShell();
                Application.Current!.Windows[0].Page = appShell;
            }
            else
            {
                IsBusy = false;
                await Shell.Current.DisplayAlert("Errore", "Credenziali non valide", "OK");
            }
        }

        [RelayCommand]
        public async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}