using MessengerServer.Model;
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
    var login = LoginEntry.Text;
    var password = PasswordEntry.Text;

    if (string.IsNullOrWhiteSpace(login))
    {
        await DisplayAlert("Ошибка", "Поле логина не может быть пустым", "OK");
        return;
    }

    if (string.IsNullOrWhiteSpace(password))
    {
        await DisplayAlert("Ошибка", "Поле пароля не может быть пустым", "OK");
        return;
    }

    var escapedLogin = Uri.EscapeDataString(login);
    var escapedPassword = Uri.EscapeDataString(password);

    var response = await _httpClient.GetAsync($"{ApiUrl}authorization?login={escapedLogin}&password={escapedPassword}");
    if (response.IsSuccessStatusCode)
    {
        var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
        await Navigation.PushAsync(new CombinedPage(user.UserId));
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
