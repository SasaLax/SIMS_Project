using System;
using System.Windows;
using CommunityHub.Application;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Service;

namespace CommunityHub.Ui.Views
{
    public partial class LogInWindow : Window
    {
        private readonly IUserService _userService;

        public LogInWindow()
        {
            InitializeComponent();
            _userService = Injector.CreateInstance<IUserService>();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            lblError.Visibility = Visibility.Collapsed;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            try
            {
                User loggedInUser = _userService.Login(email, password);
                PreusmjeriKorisnikaNaMeni(loggedInUser);
            }
            catch (InvalidOperationException ex)
            {
                lblError.Text = ex.Message;
                lblError.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom povezivanja sa bazom podataka:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreusmjeriKorisnikaNaMeni(User user)
        {
            HomeWindow homeWin = new HomeWindow(user.Id, user.Role, user.Jmbg);
            homeWin.Show();
            Close();
        }

        private void BtnOpenRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterResidentWindow
            {
                Owner = this
            };
            registerWindow.ShowDialog();
        }
    }
}