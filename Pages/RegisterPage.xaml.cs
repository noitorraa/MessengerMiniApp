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
                await DisplayAlert("������", "������� ����� ��������", "��");
                return;
            }

            string cleanedPhone = Regex.Replace(rawPhone, @"[^\d]", "");

            if (string.IsNullOrEmpty(cleanedPhone))
            {
                await DisplayAlert("������", "������� ���������� ����� ��������", "OK");
                return;
            }

            _tempPhoneNumber = cleanedPhone;

            var sendCodeResponse = await _httpClient.PostAsync(
                $"{ApiUrl}send-verification-code?phone={cleanedPhone}", null);
            if (sendCodeResponse.IsSuccessStatusCode)
            {
                await DisplayAlert("��� ���������", "������� ���������� ��� ��� ������������� ������", "OK");
            }
            else
            {
                await DisplayAlert("������", "�� ������� ��������� ���. ���������� �����.", "OK");
            }
        }

        private async void ConfirmRegistrationClicked(object sender, EventArgs e)
        {
            var code = CodeEntry.Text;
            if (string.IsNullOrWhiteSpace(code))
            {
                await DisplayAlert("������", "������� ��� �������������", "OK");
                return;
            }

            // ��������� ���, ���������� ������ �� ������
            var verifyResponse = await _httpClient.PostAsync(
                $"{ApiUrl}verify-code?phone={_tempPhoneNumber}&code={code}", null);

            if (!verifyResponse.IsSuccessStatusCode)
            {
                await DisplayAlert("������", "�������� ��� �������� ���", "OK");
                return;
            }
            else
            {
                await DisplayAlert("��� ����������", "������ ����� ��������� �����������", "OK");
                CodeEntry.IsVisible = true; // ������� ������� ������ ����� ����, � ������ ����������� ������� ��������, ����� ������������ ��� ����������, ������ ����� ���������� ���
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
                await DisplayAlert("������", "������� ������ � �������������", "��");
                return;
            }
            if (password != confirmPassword)
            {
                await DisplayAlert("������", "������ �� ���������", "��");
                return;
            }
            if (!ValidatePassword(password))
            {
                await DisplayAlert("������", "������ ������ ��������� 8-30 ��������, ��������� � �������� �����, ����� � �����������", "��");
                return;
            }
            if (string.IsNullOrEmpty(_tempPhoneNumber))
            {
                await DisplayAlert("������", "������� ������� ����� �������� � ����������� ���", "��");
                return;
            }

            // ���������� ������ ��� �����������
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
                await DisplayAlert("�����", "����������� ���������", "��");
                await Navigation.PopAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("������", error, "��");
            }
        }

        private bool ValidatePassword(string password)
        {
            // ������ ������ ��������� 8-30 ��������, ���� �� ���� ���������, ���� �������� �����, ����� � ����������
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$");
        }
    }
}
