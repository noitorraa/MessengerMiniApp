namespace MessengerMiniApp.Pages;
using MessengerServer.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

public partial class RegisterPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private string _tempPhoneNumber;
    private const string ApiUrl = "https://noitorraa-messengerserver-f42a.twc1.net/api/users/";
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

        string cleanedPhone = Regex.Replace(PhoneEntry.Text, @"[^\d]", ""); // [[1]][[5]]
        if (string.IsNullOrEmpty(cleanedPhone))
        {
            await DisplayAlert("Ошибка", "Введите корректный номер телефона", "OK");
            return;
        }

        var user = new User
        {
            Username = UsernameEntry.Text,
            PasswordHash = password, // На сервере пароль хешируется
            PhoneNumber = PhoneEntry.Text
        };

        if (!ValidatePhone(PhoneEntry.Text))
        {
            await DisplayAlert("Ошибка", "Неверный формат телефона", "OK");
            return;
        }

        if (!ValidatePassword(PasswordEntry.Text))
        {
            await DisplayAlert("Ошибка",
                "Пароль должен содержать 8-30 символов, заглавные/строчные буквы, цифры и спецсимволы",
                "OK");
            return;
        }

        _tempPhoneNumber = cleanedPhone;

        // Отправляем код
        var sendCodeResponse = await _httpClient.PostAsync(
            $"{ApiUrl}send-verification-code?phone={cleanedPhone}", null);

        if (sendCodeResponse.IsSuccessStatusCode)
        {
            await DisplayAlert("Код отправлен",
                "Введите код из SMS для завершения регистрации", "OK");
        }

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

    private bool ValidatePassword(string password)
    {
        // Требования из [[2]][[8]]
        return Regex.IsMatch(password,
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$");
    }

    private bool ValidatePhone(string phone)
    {
        // Формат из [[5]][[6]]
        return Regex.IsMatch(phone,
            @"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,15}$");
    }

    private async void ConfirmRegistrationClicked(object sender, EventArgs e)
    {
        var code = CodeEntry.Text;

        // Проверяем код
        var verifyResponse = await _httpClient.PostAsync(
            $"{ApiUrl}verify-code?phone={_tempPhoneNumber}&code={code}", null);

        if (verifyResponse.IsSuccessStatusCode)
        {
            // Теперь отправляем данные регистрации
            var user = new User
            {
                Username = UsernameEntry.Text,
                PasswordHash = PasswordEntry.Text,
                PhoneNumber = _tempPhoneNumber
            };

            var content = new StringContent(JsonConvert.SerializeObject(user),
                Encoding.UTF8, "application/json");

            var regResponse = await _httpClient.PostAsync($"{ApiUrl}registration", content);
            // ... остальная логика ...
        }
    }

    private void OnSendCodeClicked(object sender, EventArgs e)
    {

    }
}
