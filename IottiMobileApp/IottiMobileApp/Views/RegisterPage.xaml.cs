using DbMobileModel.Services;
using DbMobileModel;
using IottiMobileApp.ViewModels;

namespace IottiMobileApp.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}