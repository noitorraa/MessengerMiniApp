using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MessengerMiniApp.Pages;

public partial class RecoveryPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private const string ApiUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/api/users/";
    private string _tempPhoneNumber;

    public RecoveryPage()
    {
        InitializeComponent();
        _tempPhoneNumber = string.Empty;
    }

    // Отправка кода сброса пароля
    private async void SendCodeClicked(object sender, EventArgs e)
    {
        string rawPhone = PhoneEntry.Text;
        if (string.IsNullOrWhiteSpace(rawPhone))
        {
            await DisplayAlert("Ошибка", "Введите номер телефона", "OK");
            return;
        }

        string cleanedPhone = Regex.Replace(rawPhone, @"[^\d]", "");
        if (cleanedPhone.Length < 10 || cleanedPhone.Length > 15)
        {
            await DisplayAlert("Ошибка", "Неверный формат телефона", "OK");
            return;
        }

        _tempPhoneNumber = cleanedPhone;

        var response = await _httpClient.PostAsync($"{ApiUrl}send-reset-code?phone={cleanedPhone}", null);


        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Успех", "Код отправлен", "OK");
            ShowVerificationFields();
            StartTimer();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            await DisplayAlert("Ошибка", error, "OK");
        }
    }

    // Показать поля ввода кода и пароля
    private void ShowVerificationFields()
    {
        CodeLabel.IsVisible = true;
        CodeEntry.IsVisible = true;
        NewPasswordLabel.IsVisible = true;
        NewPassword.IsVisible = true;
        ConfirmPasswordLabel.IsVisible = true;
        ConfirmPassword.IsVisible = true;
        PasswordHint.IsVisible = true;
        ResetPasswordButton.IsVisible = true;
    }

    // Запуск таймера для повторной отправки кода
    private async void StartTimer()
    {
        int seconds = 60;
        SendCodeButton.IsEnabled = false;
        TimerLabel.IsVisible = true;

        while (seconds >= 0)
        {
            TimerLabel.Text = $"Повторить через {seconds}s";
            await Task.Delay(1000);
            seconds--;
        }

        SendCodeButton.IsEnabled = true;
        TimerLabel.IsVisible = false;
    }

    // Сброс пароля
    private async void ResetPasswordClicked(object sender, EventArgs e)
    {
        string code = CodeEntry.Text;
        string newPassword = NewPassword.Text;
        string confirmPassword = ConfirmPassword.Text;

        if (string.IsNullOrWhiteSpace(code) ||
            string.IsNullOrWhiteSpace(newPassword) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            await DisplayAlert("Ошибка", "Заполните все поля", "OK");
            return;
        }

        if (newPassword != confirmPassword)
        {
            await DisplayAlert("Ошибка", "Пароли не совпадают", "OK");
            return;
        }

        if (!ValidatePassword(newPassword))
        {
            await DisplayAlert("Ошибка", "Пароль не соответствует требованиям", "OK");
            return;
        }

        var model = new
        {
            Phone = _tempPhoneNumber,
            Code = code,
            NewPassword = newPassword
        };

        var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}reset-password", model);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Успех", "Пароль изменён", "OK");
            await Navigation.PopAsync();
            ClearFields();
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            await DisplayAlert("Ошибка", error?.Values.FirstOrDefault() ?? "Неизвестная ошибка", "OK");
        }
    }

    // Валидация пароля
    private bool ValidatePassword(string password) =>
        Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$");

    // Очистка полей ввода
    private void ClearFields()
    {
        PhoneEntry.Text = string.Empty;
        CodeEntry.Text = string.Empty;
        NewPassword.Text = string.Empty;
        ConfirmPassword.Text = string.Empty;
        ErrorLabel.IsVisible = false;
    }
}
