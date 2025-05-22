using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
namespace MessengerMiniApp.Pages;

public partial class RecoveryPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private const string ApiUrl = "https://noitorraa-messengerserver-c2cc.twc1.net/api/users/";
	public RecoveryPage()
	{
		InitializeComponent();
	}

    // RecoveryPage.xaml.cs
    private async void SendCodeClicked(object sender, EventArgs e)
    {
        var response = await _httpClient.PostAsync(
            $"{ApiUrl}send-reset-code?phone={PhoneEntry.Text}", null);

        if (response.IsSuccessStatusCode)
            await DisplayAlert("Success", "Code sent", "OK");
        else await DisplayAlert("������", "��������� ������������ ������ �������� � ��������������, ��� ��� ������� ��������������� �� ���� ����� ��������", "OK");
    }

    private async void ResetPasswordClicked(object sender, EventArgs e)
    {
        if (!ValidatePassword(NewPassword.Text) || string.IsNullOrEmpty(NewPassword.Text))
        {
            await DisplayAlert("������", "������ �� ����� ���� ������ � ������ ��������� 8-30 ��������, ��������� � �������� �����, ����� � �����������", "��");
            return;
        }
        var model = new
        {
            Phone = PhoneEntry.Text,
            Code = CodeEntry.Text,
            NewPassword = NewPassword.Text
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{ApiUrl}reset-password", model);

        if (response.IsSuccessStatusCode)
            await Navigation.PopAsync();
    }
    private bool ValidatePassword(string password)
    {
        // ������ ������ ��������� 8-30 ��������, ���� �� ���� ���������, ���� �������� �����, ����� � ����������
        return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$");
    }
}