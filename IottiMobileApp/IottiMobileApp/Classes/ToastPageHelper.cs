using System.Reflection;

namespace IottiMobileApp.Classes
{
    /// <summary>
    /// Helper per gestire il caricamento delle pagine e il toast
    /// </summary>
    public static class ToastPageHelper
    {
        /// <summary>
        /// Configura il gesture per chiudere il toast al tocco
        /// </summary>
        public static void ConfigureToastTapGesture(ContentPage page, IToastService toastService)
        {
            try
            {
                var method = typeof(TemplatedPage).GetMethod("GetTemplateChild",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (method != null)
                {
                    var toastBorder = method.Invoke(page, new[] { "ToastBorder" }) as Border;

                    if (toastBorder != null)
                    {
                        // Rimuovi gesture esistenti
                        toastBorder.GestureRecognizers.Clear();

                        // Aggiungi nuovo gesture che usa il ToastService
                        var tapGesture = new TapGestureRecognizer();
                        tapGesture.Tapped += async (s, e) =>
                        {
                            if (toastService is ToastService service)
                            {
                                await service.HideToastOnTapAsync(toastBorder, page);
                            }
                        };

                        toastBorder.GestureRecognizers.Add(tapGesture);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore configurazione gesture: {ex.Message}");
            }
        }

        /// <summary>
        /// Mostra un toast di test per verificare il funzionamento
        /// </summary>
        public static async Task ShowTestToastAsync(ContentPage page, IToastService toastService)
        {
            await toastService.ShowInfoAsync($"Toast di test da {page.GetType().Name}");
        }
    }
}