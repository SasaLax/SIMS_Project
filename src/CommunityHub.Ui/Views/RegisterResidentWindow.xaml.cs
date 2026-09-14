using System;
using System.Windows;
using CommunityHub.Application;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Service;

namespace CommunityHub.Ui.Views
{
    public partial class RegisterResidentWindow : Window
    {
        private readonly IUserService _userService;

        public RegisterResidentWindow()
        {
            InitializeComponent();
            _userService = Injector.CreateInstance<IUserService>();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string jmbg = txtJmbg.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = pwdPassword.Password;
            string name = txtName.Text.Trim();
            string surname = txtSurname.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(jmbg) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Popunite obavezna polja (JMBG, email, lozinka).", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = new User(0, jmbg, email, password, name, surname, phone, UserRole.Resident);

            try
            {
                long newId = _userService.RegisterResident(user);
                MessageBox.Show("Registracija uspešna. ID: " + newId, "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}