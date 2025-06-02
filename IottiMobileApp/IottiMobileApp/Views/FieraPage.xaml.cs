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

        // Non è più necessario il warm-up, il nuovo SmartExpander è autosufficiente
        System.Diagnostics.Debug.WriteLine("FieraPage: SmartExpander trovato e pronto");
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
        if (_smartExpander != null && _smartExpander.IsExpanded)
        {
            await _smartExpander.CloseExpanderSafely();
        }
    }
}