using System.Collections.Concurrent;
using System.Reflection;
using IottiMobileApp.Enums;
using IottiMobileApp.FontCodes;
using IottiMobileApp.Classes;

namespace IottiMobileApp.Classes
{
    public interface IToastService
    {
        Task ShowInfoAsync(string message);
        Task ShowSuccessAsync(string message);
        Task ShowWarningAsync(string message);
        Task ShowErrorAsync(string message);
        Task WaitForPageLoadedAsync(ContentPage page);
    }

    public class ToastService : IToastService
    {
        private readonly ConcurrentQueue<ToastRequest> _toastQueue = new();
        private readonly Dictionary<ContentPage, bool> _pageLoadedStatus = new();
        private readonly HashSet<ContentPage> _initializedPages = new();
        private readonly Dictionary<ContentPage, ToastState> _activeToasts = new();
        private bool _isProcessingQueue = false;
        private readonly SemaphoreSlim _queueSemaphore = new(1, 1);
        private ContentPage? _currentPage = null;

        public async Task ShowInfoAsync(string message)
        {
            await EnqueueToastAsync(message, ToastType.Info);
        }

        public async Task ShowSuccessAsync(string message)
        {
            await EnqueueToastAsync(message, ToastType.Success);
        }

        public async Task ShowWarningAsync(string message)
        {
            await EnqueueToastAsync(message, ToastType.Warning);
        }

        public async Task ShowErrorAsync(string message)
        {
            await EnqueueToastAsync(message, ToastType.Error);
        }

        public async Task WaitForPageLoadedAsync(ContentPage page)
        {
            if (!_pageLoadedStatus.ContainsKey(page))
            {
                _pageLoadedStatus[page] = false;

                // Delay ridotto per essere più reattivo
                await Task.Delay(200); // Ridotto da 500ms

                // Attende che tutti i binding siano completati
                page.Loaded += (s, e) => _pageLoadedStatus[page] = true;

                // Se non è già loaded, aspetta l'evento
                if (!_pageLoadedStatus[page])
                {
                    var tcs = new TaskCompletionSource<bool>();

                    void OnLoaded(object sender, EventArgs e)
                    {
                        page.Loaded -= OnLoaded;
                        _pageLoadedStatus[page] = true;
                        tcs.SetResult(true);
                    }

                    page.Loaded += OnLoaded;

                    // Timeout di sicurezza ridotto
                    await Task.WhenAny(tcs.Task, Task.Delay(2000)); // Ridotto da 3000ms
                }
            }

            // Aspetta fino a quando la pagina è pronta
            while (!_pageLoadedStatus.GetValueOrDefault(page, false))
            {
                await Task.Delay(50); // Ridotto da 100ms
            }

            // DELAY RIDOTTO: 200ms invece di 500ms per essere più reattivo
            await Task.Delay(200);

            System.Diagnostics.Debug.WriteLine($"✅ Pagina {page.GetType().Name} completamente pronta per toast");
        }

        private async Task EnqueueToastAsync(string message, ToastType type)
        {
            var currentPage = GetCurrentPage();
            if (currentPage == null) return;

            // CONTROLLO CAMBIO PAGINA: Se la pagina è cambiata, svuota la coda
            if (_currentPage != null && _currentPage != currentPage)
            {
                await ClearQueueForPageChangeAsync();
            }
            _currentPage = currentPage;

            // Inizializza automaticamente la pagina se non è già stata inizializzata
            await EnsurePageInitializedAsync(currentPage);

            var request = new ToastRequest
            {
                Message = message,
                Type = type,
                Page = currentPage,
                Timestamp = DateTime.Now
            };

            _toastQueue.Enqueue(request);

            System.Diagnostics.Debug.WriteLine($"🔄 Toast enqueued: {message} (Queue size: {_toastQueue.Count})");

            _ = ProcessQueueAsync();
        }

        private async Task ClearQueueForPageChangeAsync()
        {
            System.Diagnostics.Debug.WriteLine($"🔄 Cambio pagina rilevato - svuotamento coda");

            // Svuota la coda
            while (_toastQueue.TryDequeue(out var _)) { }

            // Nascondi eventuali toast attivi sulla pagina precedente
            if (_currentPage != null && _activeToasts.ContainsKey(_currentPage))
            {
                var activeToast = _activeToasts[_currentPage];
                if (activeToast.IsActive && activeToast.ToastBorder.IsVisible)
                {
                    activeToast.ToastBorder.IsVisible = false;
                    SetToastInactive(_currentPage);
                }
            }

            // Reset dello stato di processamento
            _isProcessingQueue = false;

            System.Diagnostics.Debug.WriteLine($"✅ Coda svuotata per cambio pagina");
        }

        private async Task EnsurePageInitializedAsync(ContentPage page)
        {
            if (_initializedPages.Contains(page)) return;

            System.Diagnostics.Debug.WriteLine($"🔧 Inizializzazione automatica per {page.GetType().Name}");

            // Segna come inizializzata per evitare inizializzazioni multiple
            _initializedPages.Add(page);

            // Esegui l'inizializzazione
            await InitializeToastForPageAsync(page);

            // Pulisci quando la pagina viene distrutta
            page.Unloaded += (s, e) =>
            {
                _initializedPages.Remove(page);
                _pageLoadedStatus.Remove(page);
                _activeToasts.Remove(page);

                // Se era la pagina corrente, resetta
                if (_currentPage == page)
                {
                    _currentPage = null;
                }

                System.Diagnostics.Debug.WriteLine($"🧹 Cleanup per {page.GetType().Name}");
            };
        }

        private async Task WaitForActiveToastCompletionAsync(ContentPage page)
        {
            // Controlla se c'è un toast attivo su questa pagina
            if (_activeToasts.ContainsKey(page))
            {
                var currentToast = _activeToasts[page];
                System.Diagnostics.Debug.WriteLine($"⏳ Aspettando completamento toast attivo su {page.GetType().Name}");

                // Aspetta che il toast corrente finisca
                while (_activeToasts.ContainsKey(page) &&
                       _activeToasts[page].IsActive &&
                       _activeToasts[page].StartTime == currentToast.StartTime)
                {
                    await Task.Delay(50); // Check ogni 50ms
                }

                System.Diagnostics.Debug.WriteLine($"✅ Toast precedente completato su {page.GetType().Name}");
            }
        }

        private void SetToastActive(ContentPage page, Border toastBorder)
        {
            _activeToasts[page] = new ToastState
            {
                IsActive = true,
                StartTime = DateTime.Now,
                ToastBorder = toastBorder
            };

            System.Diagnostics.Debug.WriteLine($"🟢 Toast attivato su {page.GetType().Name}");
        }

        private void SetToastInactive(ContentPage page)
        {
            if (_activeToasts.ContainsKey(page))
            {
                _activeToasts[page].IsActive = false;
                System.Diagnostics.Debug.WriteLine($"🔴 Toast disattivato su {page.GetType().Name}");
            }
        }

        private async Task InitializeToastForPageAsync(ContentPage page)
        {
            // Attende il caricamento della pagina
            await WaitForPageLoadedAsync(page);

            // Configura gesture per chiusura toast al tocco usando ToastPageHelper
            ToastPageHelper.ConfigureToastTapGesture(page, this);

            System.Diagnostics.Debug.WriteLine($"✅ Pagina {page.GetType().Name} inizializzata per toast");
        }

        /// <summary>
        /// Metodo pubblico per nascondere toast al tocco, chiamato da ToastPageHelper
        /// </summary>
        public async Task HideToastOnTapAsync(Border toastBorder, ContentPage page)
        {
            if (!toastBorder.IsVisible) return;

            System.Diagnostics.Debug.WriteLine("👆 Toast toccato - chiusura rapida");

            // SEGNA COME INATTIVO IMMEDIATAMENTE
            SetToastInactive(page);

            // ANIMAZIONE DI CHIUSURA RAPIDA AL TOCCO (più veloce della normale)
            var hideAnimation = new Animation();

            // Scale down rapido
            hideAnimation.Add(0, 0.4, new Animation(v => toastBorder.Scale = v, toastBorder.Scale, 0.7, Easing.CubicIn));

            // Slide down rapido
            hideAnimation.Add(0.1, 0.8, new Animation(v => toastBorder.TranslationY = v, toastBorder.TranslationY, 60, Easing.CubicIn));

            // Fade out rapido
            hideAnimation.Add(0.3, 1, new Animation(v => toastBorder.Opacity = v, toastBorder.Opacity, 0, Easing.CubicIn));

            hideAnimation.Commit(toastBorder, "ToastHideOnTap", 16, 300, finished: (v, c) =>
            {
                toastBorder.IsVisible = false;
                toastBorder.TranslationY = 0;
                toastBorder.Scale = 1;
                toastBorder.Opacity = 0;
                System.Diagnostics.Debug.WriteLine("✅ Toast chiuso al tocco");
            });
        }

        private async Task ProcessQueueAsync()
        {
            await _queueSemaphore.WaitAsync();

            try
            {
                if (_isProcessingQueue) return;

                _isProcessingQueue = true;

                while (_toastQueue.TryDequeue(out var request))
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 Processing toast: {request.Message} (Queue remaining: {_toastQueue.Count})");

                    // Verifica che la richiesta sia ancora per la pagina corrente
                    if (_currentPage != request.Page)
                    {
                        System.Diagnostics.Debug.WriteLine($"🚫 Toast scartato - pagina cambiata: {request.Message}");
                        continue;
                    }

                    // Aspetta che la pagina sia caricata
                    await WaitForPageLoadedAsync(request.Page);

                    // CONTROLLO DINAMICO: Aspetta che eventuali toast attivi sulla stessa pagina finiscano
                    await WaitForActiveToastCompletionAsync(request.Page);

                    // Verifica di nuovo che sia ancora la pagina corrente
                    if (_currentPage != request.Page)
                    {
                        System.Diagnostics.Debug.WriteLine($"🚫 Toast scartato durante attesa - pagina cambiata: {request.Message}");
                        continue;
                    }

                    // Mostra il toast
                    await ShowToastInPageAsync(request);

                    System.Diagnostics.Debug.WriteLine($"🔄 Toast completato: {request.Message}");
                }

                System.Diagnostics.Debug.WriteLine($"✅ Coda processata completamente (Queue size: {_toastQueue.Count})");
            }
            finally
            {
                _isProcessingQueue = false;
                _queueSemaphore.Release();
            }
        }

        private async Task ShowToastInPageAsync(ToastRequest request)
        {
            try
            {
                // Usa reflection per accedere agli elementi del template
                var method = typeof(TemplatedPage).GetMethod("GetTemplateChild",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (method == null) return;

                var toastBorder = method.Invoke(request.Page, new[] { "ToastBorder" }) as Border;
                var messageLabel = method.Invoke(request.Page, new[] { "MessageLabel" }) as Label;
                var iconBorder = method.Invoke(request.Page, new[] { "IconBorder" }) as Border;
                var iconLabel = method.Invoke(request.Page, new[] { "IconLabel" }) as Label;

                if (toastBorder == null || messageLabel == null) return;

                await request.Page.Dispatcher.DispatchAsync(async () =>
                {
                    // SEGNA IL TOAST COME ATTIVO
                    SetToastActive(request.Page, toastBorder);

                    // Configura aspetto
                    ConfigureToastAppearance(request.Type, iconBorder, iconLabel);
                    messageLabel.Text = request.Message;

                    // Reset stato iniziale
                    toastBorder.IsVisible = true;
                    toastBorder.Opacity = 0;
                    toastBorder.TranslationY = 50; // Parte dal basso
                    toastBorder.Scale = 0.8; // Parte più piccolo

                    // ANIMAZIONE DI ENTRATA FLUIDA (più veloce)
                    var entranceAnimation = new Animation();

                    // Fade in
                    entranceAnimation.Add(0, 0.6, new Animation(v => toastBorder.Opacity = v, 0, 1));

                    // Slide up
                    entranceAnimation.Add(0, 0.8, new Animation(v => toastBorder.TranslationY = v, 50, 0, Easing.CubicOut));

                    // Scale up
                    entranceAnimation.Add(0.2, 1, new Animation(v => toastBorder.Scale = v, 0.8, 1, Easing.SpringOut));

                    // Esegui animazione di entrata (ridotta da 600ms a 400ms)
                    entranceAnimation.Commit(request.Page, "ToastEntrance", 16, 400);

                    // Aspetta che l'animazione di entrata finisca
                    await Task.Delay(400);

                    System.Diagnostics.Debug.WriteLine($"✅ Toast mostrato: {request.Message}");

                    // Aspetta 3 secondi prima di nascondere (ridotto da 4)
                    await Task.Delay(3000);

                    // Controlla se il toast è ancora attivo (non chiuso manualmente)
                    if (_activeToasts.ContainsKey(request.Page) && _activeToasts[request.Page].IsActive)
                    {
                        // ANIMAZIONE DI USCITA FLUIDA (inversa a quella di entrata)
                        var exitAnimation = new Animation();

                        // Scale down (inverso di scale up)
                        exitAnimation.Add(0, 0.3, new Animation(v => toastBorder.Scale = v, 1, 0.8, Easing.CubicIn));

                        // Slide down (inverso di slide up)
                        exitAnimation.Add(0.2, 1, new Animation(v => toastBorder.TranslationY = v, 0, 50, Easing.CubicIn));

                        // Fade out (inverso di fade in)
                        exitAnimation.Add(0.4, 1, new Animation(v => toastBorder.Opacity = v, 1, 0, Easing.CubicIn));

                        // Esegui animazione di uscita (ridotta da 600ms a 400ms)
                        exitAnimation.Commit(request.Page, "ToastExit", 16, 400, finished: (v, c) =>
                        {
                            toastBorder.IsVisible = false;
                            toastBorder.TranslationY = 0;
                            toastBorder.Scale = 1;
                            toastBorder.Opacity = 0;

                            // SEGNA IL TOAST COME INATTIVO
                            SetToastInactive(request.Page);

                            System.Diagnostics.Debug.WriteLine($"🔽 Toast nascosto automaticamente: {request.Message}");
                        });
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"🔽 Toast già chiuso manualmente: {request.Message}");
                        SetToastInactive(request.Page);
                    }
                });
            }
            catch (Exception ex)
            {
                SetToastInactive(request.Page);
                System.Diagnostics.Debug.WriteLine($"💥 Errore in ShowToastInPageAsync: {ex.Message}");
            }
        }

        private void ConfigureToastAppearance(ToastType type, Border iconBorder, Label iconLabel)
        {
            if (iconBorder == null || iconLabel == null) return;

            switch (type)
            {
                case ToastType.Info:
                    iconBorder.BackgroundColor = Color.FromArgb("#2196F3");
                    iconLabel.Text = FASolid.Info;
                    break;
                case ToastType.Success:
                    iconBorder.BackgroundColor = Color.FromArgb("#4CAF50");
                    iconLabel.Text = FASolid.Check;
                    break;
                case ToastType.Warning:
                    iconBorder.BackgroundColor = Color.FromArgb("#FF9800");
                    iconLabel.Text = FASolid.TriangleExclamation;
                    break;
                case ToastType.Error:
                    iconBorder.BackgroundColor = Color.FromArgb("#F44336");
                    iconLabel.Text = FASolid.Xmark;
                    break;
            }
        }

        private ContentPage? GetCurrentPage()
        {
            if (Application.Current?.Windows?.Count > 0)
            {
                var window = Application.Current.Windows[0];

                if (window.Page is Shell shell && shell.CurrentPage is ContentPage page)
                    return page;

                if (window.Page is NavigationPage navPage && navPage.CurrentPage is ContentPage currentPage)
                    return currentPage;

                return window.Page as ContentPage;
            }
            return null;
        }

        private class ToastRequest
        {
            public string Message { get; set; } = string.Empty;
            public ToastType Type { get; set; }
            public ContentPage Page { get; set; } = null!;
            public DateTime Timestamp { get; set; }
        }

        private class ToastState
        {
            public bool IsActive { get; set; }
            public DateTime StartTime { get; set; }
            public Border ToastBorder { get; set; } = null!;
        }
    }
}