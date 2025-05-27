using IottiMobileApp.ViewModels;
using IottiMobileApp.Controls;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core;

namespace IottiMobileApp.Views;

public partial class FieraPage : ContentPage
{
    private readonly FieraViewModel _viewModel;
    private SmartExpander? _smartExpander;

    public FieraPage(FieraViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;

        // Disabilita la freccia indietro e mantiene solo l'hamburger menu
        NavigationPage.SetHasBackButton(this, false);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Ricarica dalla sessione quando la pagina appare
        _viewModel.LoadFromSession();

        // Trova il SmartExpander nella pagina
        _smartExpander = this.FindByName<SmartExpander>("SmartFieraExpander");

        // Warm-up animazioni solo se necessario
        if (_smartExpander?.InternalExpander.Content is VisualElement content && content.IsVisible)
        {
            // Warm-up: animazione di 1ms per traduzione + dissolvenza
            _ = content.TranslateTo(0, 0, 1);
            _ = content.FadeTo(content.Opacity, 1);
        }
    }

    // Gestisce la selezione - ora viene chiamato dal SmartExpander
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Il SmartExpander gestisce già la chiusura automatica,
        // qui puoi aggiungere logica aggiuntiva se necessario
        if (e.CurrentSelection?.Count > 0)
        {
            // Logica aggiuntiva dopo la selezione (es. navigazione, etc.)
            // La chiusura dell'expander è gestita automaticamente dal SmartExpander

            System.Diagnostics.Debug.WriteLine($"Elemento selezionato: {_viewModel.FieraSelezionata?.MteSapProjectName}");
        }
    }

    // Gestisce tap esterni per chiudere l'expander
    private async void OnPageTapped(object? sender, EventArgs e)
    {
        if (_smartExpander != null && _smartExpander.InternalExpander.IsExpanded)
        {
            await _smartExpander.CloseExpanderSafely();
        }
    }

    // Metodi helper per controllare l'animazione dell'expander
    private bool IsExpanderAnimating()
    {
        if (_smartExpander == null) return false;

        var animationBehavior = _smartExpander.InternalExpander.Behaviors
            .OfType<IottiMobileApp.Behaviors.ExpanderAnimationBehavior>()
            .FirstOrDefault();
        return animationBehavior?.IsAnimating ?? false;
    }

    // Metodo per gestire programmaticamente l'apertura/chiusura in modo sicuro
    public async Task ToggleExpanderSafely(bool expand)
    {
        if (_smartExpander == null) return;

        var expander = _smartExpander.InternalExpander;

        // Non fare nulla se l'expander è già nello stato richiesto
        if (expander.IsExpanded == expand)
            return;

        var animationBehavior = expander.Behaviors
            .OfType<IottiMobileApp.Behaviors.ExpanderAnimationBehavior>()
            .FirstOrDefault();

        if (animationBehavior != null)
        {
            // Attendi che eventuali animazioni finiscano
            var maxWait = TimeSpan.FromMilliseconds(800);
            var checkInterval = TimeSpan.FromMilliseconds(50);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (stopwatch.Elapsed < maxWait)
            {
                // Controlla se l'animazione è ancora in corso
                if (!IsExpanderAnimating())
                    break;
                await Task.Delay(checkInterval);
            }
        }

        // Cambia stato solo se non è già in animazione
        if (!IsExpanderAnimating())
        {
            expander.IsExpanded = expand;
        }
    }

    // Metodi pubblici per aprire/chiudere da codice esterno
    public async Task OpenExpander() => await ToggleExpanderSafely(true);
    public async Task CloseExpander() => await ToggleExpanderSafely(false);
    public async Task ToggleExpander()
    {
        if (_smartExpander != null)
        {
            await ToggleExpanderSafely(!_smartExpander.InternalExpander.IsExpanded);
        }
    }
}