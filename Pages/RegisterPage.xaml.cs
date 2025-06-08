using MessengerServer.Model;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace MessengerMiniApp.Pages
{
    public partial class RegisterPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private string? _tempPhoneNumber;
        private const string ApiUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/api/users/";

        public RegisterPage()
        {
            InitializeComponent();
            regBtn.IsEnabled = false;
            CodeEntry.IsVisible = false;
            ConfirmBtn.IsVisible = false;
        }

        private async void OnSendCodeClicked(object sender, EventArgs e)
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
            Console.WriteLine($"Очищенный номер телефона: {_tempPhoneNumber}");

            var response = await _httpClient.PostAsync($"{ApiUrl}send-verification-code",
                new StringContent(JsonConvert.SerializeObject(new { phone = cleanedPhone }), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Успех", "Код отправлен", "OK");
                CodeEntry.IsVisible = true;
                ConfirmBtn.IsVisible = true;
                SendCodeButton.IsVisible = false;
            }
            else
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var error = Encoding.UTF8.GetString(bytes);
                await DisplayAlert("Ошибка", error, "OK");
            }
        }

        private async void ConfirmRegistrationClicked(object sender, EventArgs e)
        {
            var code = CodeEntry.Text;
            if (string.IsNullOrWhiteSpace(code))
            {
                await DisplayAlert("Ошибка", "Введите код подтверждения", "OK");
                return;
            }

            var content = new StringContent(JsonConvert.SerializeObject(new { phone = _tempPhoneNumber, code }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{ApiUrl}verify-code", content);

            if (!response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var error = Encoding.UTF8.GetString(bytes);
                await DisplayAlert("Ошибка", error, "OK");
                return;
            }

            await DisplayAlert("Успех", "Код подтверждён", "OK");
            CodeEntry.IsVisible = false;
            ConfirmBtn.IsVisible = false;
            regBtn.IsEnabled = true;
        }

        private void OnBackClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {

            var password = PasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlert("Ошибка", "Заполните поля пароля и подтверждения пароля", "Ок");
                return;
            }
            if (password != confirmPassword)
            {
                await DisplayAlert("Ошибка", "Пароли не совпадают", "Ок");
                return;
            }
            if (!ValidatePassword(password))
            {
                await DisplayAlert("Ошибка", "Пароль должен быть длиной от 8 до 30 символов, содержать цифры и сценсимволы", "Ок");
                return;
            }
            if (string.IsNullOrEmpty(_tempPhoneNumber))
            {
                await DisplayAlert("Ошибка", "Телефон не подтверждён", "Ок");
                return;
            }

            var user = new User
            {
                Username = LoginEntry.Text,
                PasswordHash = password,
                PhoneNumber = _tempPhoneNumber
            };

            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{ApiUrl}registration", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Успешно", "Вы зарегистрировались", "Ок");
                await Navigation.PopAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", error, "Ок");
            }
        }

        private bool ValidatePassword(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$");
        }
    }
}
