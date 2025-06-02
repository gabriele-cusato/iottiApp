using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DbMobileModel.Services;
using DbMobileModel.Services.Interfaces;
using IottiMobileApp.Classes;

namespace IottiMobileApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IRemoteAuthService _authService;
        private readonly IToastService _toastService;

        [ObservableProperty]
        public bool? isRefreshing;
        [ObservableProperty]
        public float? pageHeight;

        public MainViewModel(IRemoteAuthService authService, IToastService toastService)
        {
            _authService = authService;
            _toastService = toastService;
        }

        /// <summary>
        /// Metodo chiamato quando la pagina viene caricata
        /// </summary>
        public async Task OnPageLoadedAsync()
        {
            // Esempio: Toast di benvenuto
            var username = Preferences.Get("Username", "Utente");
            System.Diagnostics.Debug.WriteLine($"DEBUG: Tentativo di mostrare toast per {username}");

            await _toastService.ShowSuccessAsync("Prodotto salvato con successo!");

            System.Diagnostics.Debug.WriteLine($"DEBUG: ShowInfoAsync completato");
        }
    }
}