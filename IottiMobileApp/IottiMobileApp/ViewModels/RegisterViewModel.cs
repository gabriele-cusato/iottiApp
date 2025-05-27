using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbMobileModel.DTO;
using DbMobileModel.Services.Interfaces;
using IottiMobileApp.Views;

namespace IottiMobileApp.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IRemoteAuthService _authService;

        public RegisterViewModel(IRemoteAuthService authService)
        {
            _authService = authService;
        }

        [ObservableProperty] private string? nome;
        [ObservableProperty] private string? cognome;
        [ObservableProperty] private string? username;
        [ObservableProperty] private string? emailField;
        [ObservableProperty] private string? password;

        public IRelayCommand CustomBackCommand => new RelayCommand(async () =>
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        });

        [RelayCommand]
        public async Task RegisterClicked()
        {
            // Validazione minima
            //if (string.IsNullOrWhiteSpace(Nome) ||
            //    string.IsNullOrWhiteSpace(Cognome) ||
            //    string.IsNullOrWhiteSpace(Username) ||
            //    string.IsNullOrWhiteSpace(Email) ||
            //    string.IsNullOrWhiteSpace(Password))
            //{
            //    await Shell.Current.DisplayAlert("Errore",
            //        "Compila tutti i campi obbligatori",
            //        "OK");
            //    return;
            //}

            //// Legge e normalizza i valori nei campi
            //var dto = new RegisterDto
            //{
            //    nome = Nome!.Trim() ?? string.Empty,
            //    cognome = Cognome!.Trim() ?? string.Empty,
            //    username = Username!.Trim() ?? string.Empty,
            //    email = Email!.Trim() ?? string.Empty,
            //    password = Password! ?? string.Empty
            //};

            //// 3) Chiamata al servizio con DTO
            //bool created = await _authService.RegisterAsync(dto);

            //// 4) Esito
            //if (created)
            //{
            //    await Shell.Current.DisplayAlert("Successo",
            //        "Registrazione completata",
            //        "OK");
            //    await Shell.Current.GoToAsync("login");
            //}
            //else
            //{
            //    await Shell.Current.DisplayAlert("Errore",
            //        "Username o email già esistenti",
            //        "OK");
            //}
        }
    }
}
