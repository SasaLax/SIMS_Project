using System;
using System.Windows;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;

namespace CommunityHub.Ui.Views
{
    public partial class LogInWindow : Window
    {
        private readonly UserDbRepository _userRepository;

        public LogInWindow()
        {
            InitializeComponent();
            _userRepository = new UserDbRepository();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            lblError.Visibility = Visibility.Collapsed;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Molimo unesite email i lozinku.";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                long? userId = _userRepository.GetIdByCredentials(email, password);

                if (userId == null)
                {
                    lblError.Text = "Pogrešan email ili lozinka!";
                    lblError.Visibility = Visibility.Visible;
                }
                else
                {
                    User? loggedInUser = _userRepository.GetById(userId.Value);

                    if (loggedInUser != null)
                    {
                        PreusmjeriKorisnikaNaMeni(loggedInUser);
                    }
                    else
                    {
                        lblError.Text = "Greška pri učitavanju profila korisnika.";
                        lblError.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom povezivanja sa bazom podataka:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreusmjeriKorisnikaNaMeni(User user)
        {
            HomeWindow homeWin = new HomeWindow(user.Id, user.Role);
            homeWin.Show();
            this.Close();
        }

        // New handler: open register window
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