using Core.ViewModels.Dto.Auth.Auth;
using Lead.Mobile.Interfaces;
using System.Windows.Input;

namespace Lead.Mobile.ViewModels;

public class LoginViewModel : BindableObject
{
    private readonly IHttpService _httpService;

    private string _email;
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    private string _password;
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel(IHttpService httpService)
    {
        _httpService = httpService;
        LoginCommand = new Command(async () => await LoginAsync());
    }

    private async Task LoginAsync()
    {
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please enter both email and password.", "OK");
            return;
        }

        try
        {
            var loginRequest = new LoginDto
            {
                Email = Email,
                Password = Password
            };

            var response = await _httpService.PostAsync<AuthResponseDto>(
                "v1",          // API version
                "auth/login",  // API endpoint
                loginRequest
            );

            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                // Save token & role
                _httpService.SetBearerToken(response.Token);
                await SecureStorage.SetAsync("AuthToken", response.Token);


                // Navigate to AppShell
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Login Failed", "Invalid credentials.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
        }
    }
}
