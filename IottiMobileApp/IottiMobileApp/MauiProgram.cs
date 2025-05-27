using System.Reflection;
using BarcodeScanning;
using DbMobileModel.Context;
using DbMobileModel.Services;
using EncryptModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IottiMobileApp.Views;

using DbMobileModel;
using IottiMobileApp.Classes;

namespace IottiMobileApp
{
    public static class MauiProgram
    {
        internal static ConfigParams AppConfig { get; private set; } = null!;

        /// <summary>
        /// metodo principale dell'app maui, rappresenta l'entry point dell'applicazione
        /// </summary>
        /// <returns></returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            //configuro i parametri di config dal file json
            var configParams = new ConfigParams("AppSettings.json");
            AppConfig = configParams;
            
            //richiamo metodi BuilderRegistration
            builder
                .RegisterFonts()
                .RegisterLibraries(configParams)
                .RegisterServices(configParams)
                .RegisterViewsAndViewModels();

            //faccio in modo che le eccezioni non siano silenziose
            HookGlobalExceptionHandlers();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            var app = builder.Build();
            DbConnection.serviceProvider = app.Services;

            return app;
        }

        /// <summary>
        /// Senza questo metodo, alcune eccezioni diventano silenziose e non vengono lanciate
        /// </summary>
        private static void HookGlobalExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Console.WriteLine($"UnhandledException: {(e.ExceptionObject as Exception)}");
            };
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Console.WriteLine($"UnobservedTaskException: {e.Exception.Flatten()}");
                e.SetObserved();
            };
        }
    }
}
