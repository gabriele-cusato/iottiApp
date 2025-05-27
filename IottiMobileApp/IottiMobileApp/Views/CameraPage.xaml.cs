using BarcodeScanning;
using DbMobileModel.Services;
using DbMobileModel.Models;
namespace IottiMobileApp.Views;

public partial class CameraPage : ContentPage
{
    private bool trovato = false;

    public CameraPage()
    {
        InitializeComponent();
        askPermissions();
    }

    public async void askPermissions()
    {
        await Methods.AskForRequiredPermissionAsync();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        cameraView.CameraEnabled = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        cameraView.CameraEnabled = false;
    }

    private void CameraView_OnDetectionFinished(object sender, OnDetectionFinishedEventArg e)
    {
        if (e.BarcodeResults.Count > 0)
        {
            if (trovato)
            {
                cameraView.CameraEnabled = false;
                Navigation.PopAsync();
            }

            BarcodeResult? result = e.BarcodeResults.FirstOrDefault();

            if (result != null)
            {
                trovato = true;
            }

            if (trovato)
            {
                cameraView.CameraEnabled = false;
                Navigation.PopAsync();
            }
        }
    }
}