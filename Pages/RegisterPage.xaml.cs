using MessengerServer.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace MessengerMiniApp.Pages
{
    public partial class RegisterPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private string _tempPhoneNumber;
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
            CodeEntry.IsVisible = true;
            ConfirmBtn.IsVisible = true;
            string rawPhone = PhoneEntry.Text;

            if (string.IsNullOrWhiteSpace(rawPhone))
            {
                await DisplayAlert("Ошибка", "Введите номер телефона", "ОК");
                return;
            }

            string cleanedPhone = Regex.Replace(rawPhone, @"[^\d]", "");

            if (string.IsNullOrEmpty(cleanedPhone))
            {
                await DisplayAlert("Ошибка", "Введите корректный номер телефона", "OK");
                return;
            }

            _tempPhoneNumber = cleanedPhone;

            var sendCodeResponse = await _httpClient.PostAsync(
                $"{ApiUrl}send-verification-code?phone={cleanedPhone}", null);
            if (sendCodeResponse.IsSuccessStatusCode)
            {
                await DisplayAlert("Код отправлен", "Введите полученный код для подтверждения номера", "OK");
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось отправить код. Попробуйте позже.", "OK");
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

            // Проверяем код, отправляем запрос на сервер
            var verifyResponse = await _httpClient.PostAsync(
                $"{ApiUrl}verify-code?phone={_tempPhoneNumber}&code={code}", null);

            if (!verifyResponse.IsSuccessStatusCode)
            {
                await DisplayAlert("Ошибка", "Неверный или истекший код", "OK");
                return;
            }
            else
            {
                await DisplayAlert("Код подтверждён", "Теперь можно завершить регистрацию", "OK");
                CodeEntry.IsVisible = true; // сделаем видимой панель ввода кода, а кнопку регистрации сделаем активной, чтобы пользователь мог зарегаться, только когда подтвердит код
                ConfirmBtn.IsVisible = true;
                regBtn.IsEnabled = true;
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {

            var password = PasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlert("Ошибка", "Введите пароль и подтверждение", "ОК");
                return;
            }
            if (password != confirmPassword)
            {
                await DisplayAlert("Ошибка", "Пароли не совпадают", "ОК");
                return;
            }
            if (!ValidatePassword(password))
            {
                await DisplayAlert("Ошибка", "Пароль должен содержать 8-30 символов, заглавные и строчные буквы, цифры и спецсимволы", "ОК");
                return;
            }
            if (string.IsNullOrEmpty(_tempPhoneNumber))
            {
                await DisplayAlert("Ошибка", "Сначала укажите номер телефона и подтвердите код", "ОК");
                return;
            }

            // Подготовка данных для регистрации
            var user = new User
            {
                Username = UsernameEntry.Text,
                PasswordHash = password,
                PhoneNumber = _tempPhoneNumber
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
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", error, "ОК");
            }
        }

        private bool ValidatePassword(string password)
        {
            // Пароль должен содержать 8-30 символов, хотя бы одну заглавную, одну строчную букву, цифру и спецсимвол
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$");
        }
    }
}
