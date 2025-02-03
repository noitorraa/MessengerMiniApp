using MessengerServer.Model;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace MessengerMiniApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private const string ApiUrl = "https://noitorraa-messengerserver-7295.twc1.net/api/users/";
    public LoginPage()
	{
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var login = LoginEntry.Text;
        var password = PasswordEntry.Text;

        var response = await _httpClient.GetAsync($"{ApiUrl}authorization?login={login}&password={password}");
        if (response.IsSuccessStatusCode)
        {
            var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            await Navigation.PushAsync(new ChatListPage(user.UserId));
        }
        else
        {
            await DisplayAlert("Ошибка", "Неверный логин или пароль", "OK");
        }
    }

    // Регистрация
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}
