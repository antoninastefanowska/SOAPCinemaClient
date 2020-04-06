using CinemaClient.CinemaService;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CinemaClient
{
    public partial class LoginWindow : Window
    {
        private readonly CinemaServiceClient service;

        public LoginWindow()
        {
            InitializeComponent();
            
            service = new CinemaServiceClient();
            service.Endpoint.EndpointBehaviors.Add(new IPAttachBehavior());
        }

        private Authentication CreateAuthentication()
        {
            Authentication authentication = new Authentication();
            authentication.Username = UsernameTextbox.Text;
            authentication.Password = Encrypt(PasswordTextbox.Password.ToString());
            return authentication;
        }

        private string Encrypt(string input)
        {
            byte[] data = Encoding.ASCII.GetBytes(input);
            byte[] hashData = new SHA1Managed().ComputeHash(data);
            string hash = string.Empty;

            foreach (byte b in hashData)
                hash += b.ToString("X2");
            return hash;
        }

        private void StartLoading()
        {
            LoginButton.IsEnabled = false;
            CreateAccountButton.IsEnabled = false;
            ProgressBar.Visibility = Visibility.Visible;
        }

        private void StopLoading()
        {
            LoginButton.IsEnabled = true;
            CreateAccountButton.IsEnabled = true;
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication authentication = CreateAuthentication();
            Login(authentication);
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication authentication = CreateAuthentication();
            CreateAccount(authentication);
        }

        private async void Login(Authentication authentication)
        {
            StartLoading();
            try
            {
                await service.LoginAsync(authentication);
                SwitchWindows(authentication);
            }
            catch (FaultException e)
            {
                Utils.ShowError(this, e.Message);
            }
            StopLoading();
        }

        private async void CreateAccount(Authentication authentication)
        {
            StartLoading();
            try
            {
                CreateUserResponse response = await service.CreateUserAsync(authentication);
                Utils.ShowInfo(this, "Konto zostało utworzone.");
                SwitchWindows(authentication);
            }
            catch (FaultException e)
            {
                Utils.ShowError(this, e.Message);
            }
            StopLoading();
        }

        private void SwitchWindows(Authentication authentication)
        {
            MainWindow newWindow = new MainWindow();
            newWindow.Authentication = authentication;
            newWindow.Service = service;
            Application.Current.MainWindow = newWindow;
            Close();
            newWindow.Show();
        }
    }
}
