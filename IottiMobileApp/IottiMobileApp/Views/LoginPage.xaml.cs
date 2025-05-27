using IottiMobileApp.ViewModels;

namespace IottiMobileApp.Views;

public partial class LoginPage : ContentPage
{
    //il costruttore vuoto servirebbe perche quando si clicca su una pagina dove si vuole andare, all'interno del menu hamburger di maui,
    //non viene chiamato il normale costruttore ma viene chiamato un costruttore senza parametri.
    //per risolvere e usare l'injection anche in questo caso i command bindati devono essere scritti correttamente, nel senso che se nel 
    //view model il metodo si chiama GoToRegisterAsync, allora nella view bisogna bindare GoToRegisterCommand (tolta parola Async e aggiunto Command)
    //questo perchè è il comportamento del decoratore RelayCommand.
    //e poi nel flyout dev'essere messo del codice custom, che permette di richiamare metodi nel code-behind di AppShell, cosi da poter fare la redirect correttamente
    //tramite AppShell.Current.GoToAsync(nameof(LoginPage)); in questo modo viene chiamato il costruttore giusto. Bisogna comuqnue sempre ricordarsi di registrare le 
    //pagine e i view model all'interno del builder

    //ATTENZIONE: i metodi che iniziano con On vengono ignorati

    public LoginPage(LoginViewModel vm)               
    {  
        InitializeComponent();
        BindingContext = vm;
    }  
}