using MessengerServer.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace MessengerMiniApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private const string ApiUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/api/users/";
    public LoginPage()
	{
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var login = Uri.EscapeDataString(LoginEntry.Text);
        var password = Uri.EscapeDataString(PasswordEntry.Text);

        var response = await _httpClient.GetAsync($"{ApiUrl}authorization?login={login}&password={password}");
        if (response.IsSuccessStatusCode)
        {
            var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            await Navigation.PushAsync(new ChatListPage(user.UserId));
        }
        else
        {
            await DisplayAlert("Ошибка", "Произошла ошибка при попытке авторизации", "OK");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RecoveryPage());
    }
}
