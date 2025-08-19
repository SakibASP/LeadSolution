using Lead.Mobile.Views.Auth;

namespace Lead.Mobile.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    // Navigate to Profile Page
    private async void OnProfileClicked(object sender, EventArgs e)
    {
        // Navigate using Shell
        await Shell.Current.GoToAsync(nameof(Login));
    }

    // Logout
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // Clear stored token (SecureStorage)
        try
        {
            SecureStorage.Remove("AuthToken");
        }
        catch
        {
            // ignore if token doesn't exist
        }

        // Reset MainPage to Login
        Application.Current.MainPage = new Login();
    }
}
