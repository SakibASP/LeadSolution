using Lead.Mobile.Interfaces;
using Lead.Mobile.ViewModels;

namespace Lead.Mobile.Views.Auth;

public partial class Login : ContentPage
{

    public Login()
    {
        InitializeComponent();

        // Resolve IHttpService from DI
        var httpService = MauiProgram.Services?.GetService<IHttpService>();
        BindingContext = new LoginViewModel(httpService!);
    }

}