namespace MessengerMiniApp.Pages;
using MessengerServer.Models;
using Newtonsoft.Json;
using System.Text;

public partial class RegisterPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private const string ApiUrl = "https://noitorraa-messengerserver-ba27.twc1.net/api/users/";
    public RegisterPage()
	{
		InitializeComponent();
	}

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var password = PasswordEntry.Text;
        var confirmPassword = ConfirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
        {
            await DisplayAlert("Ошибка", "Пожалуйста, введите пароль и подтвердите его", "ОК");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Ошибка", "Пароли не совпадают", "ОК");
            return;
        }

        var user = new User
        {
            Username = UsernameEntry.Text,
            PasswordHash = password // На сервере пароль хешируется
        };

        var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{ApiUrl}registration", content);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Успех", "Регистрация завершена", "ОК");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Ошибка", "Логин уже занят", "ОК");
        }
    }

}
