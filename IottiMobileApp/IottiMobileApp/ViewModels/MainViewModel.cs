using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DbMobileModel.Services;
using DbMobileModel.Services.Interfaces;

namespace IottiMobileApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IRemoteAuthService _authService;

        [ObservableProperty]
        public bool? isRefreshing;

        [ObservableProperty]
        public float? pageHeight;

        public MainViewModel(IRemoteAuthService authService)
        {
            _authService = authService;
        }
    }
}
