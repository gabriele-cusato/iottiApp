using BarcodeScanning;
using DbMobileModel.Context;
using DbMobileModel.Services;
using EncryptModule;
using Microsoft.EntityFrameworkCore;
using IottiMobileApp.ViewModels;
using IottiMobileApp.Views;
using DbMobileModel.Services.Interfaces;

using CommunityToolkit.Maui;

namespace IottiMobileApp.Classes
{
    internal static class BuilderRegistration
    {
        public static MauiAppBuilder RegisterLibraries(this MauiAppBuilder builder, ConfigParams config)
        {
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseBarcodeScanning();

            builder.Services.AddSingleton(config);
            builder.Services.AddSingleton<UserSession>();

            return builder;
        }

        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder, ConfigParams config)
        {
            builder.Services
                .AddDbContext<LocalDbContext>(opt =>
                    opt.UseSqlite(config.LocalServerConnection))
                .AddDbContext<IntermediateDbContext>(opt =>
                    opt.UseSqlServer(config.IntermediateServerConnection))
                .AddSingleton<IPasswordHasher, EncryptionHelper>()
                .AddScoped<IRemoteAuthService, RemoteAuthService>()
                .AddScoped<ILocalDbService, LocalDbService>()
                .AddScoped<IIntermediateDbService, IntermediateDbService>();

            return builder;
        }

        public static MauiAppBuilder RegisterViewsAndViewModels(this MauiAppBuilder builder)
        {
            builder.Services
                .AddPageWithViewModel<MainPage, MainViewModel>()
                .AddPageWithViewModel<LoginPage, LoginViewModel>()
                .AddPageWithViewModel<RegisterPage, RegisterViewModel>()
                .AddPageWithViewModel<FieraPage, FieraViewModel>();

            return builder;
        }

        public static MauiAppBuilder RegisterFonts(this MauiAppBuilder builder)
        {
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont("fa-solid-900.ttf", "FASolid");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

            return builder;
        }
    }
}
