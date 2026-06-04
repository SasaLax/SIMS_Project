using System;
using System.Text.RegularExpressions;
using System.Windows;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;

namespace CommunityHub.Ui.Views
{
    public partial class RegisterResidentWindow : Window
    {
        public RegisterResidentWindow()
        {
            InitializeComponent();
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

            // Basic validation
            if (string.IsNullOrEmpty(jmbg) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Popunite obavezna polja (JMBG, email, lozinka).", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(jmbg, @"^\d{13}$"))
            {
                MessageBox.Show("JMBG mora sadržati tačno 13 cifara.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!email.Contains("@"))
            {
                MessageBox.Show("Unesite validan email.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Lozinka mora imati najmanje 6 znakova.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(phone, @"^\+387\d{8}$"))
            {
                MessageBox.Show("Mobilni broj mora početi sa +387 i imati 8 cifara nakon toga (npr. +38765111222).", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var userRepo = new UserDbRepository();

            // Uniqueness checks
            if (userRepo.EmailExists(email))
            {
                MessageBox.Show("Email već postoji.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (userRepo.JmbgExists(jmbg))
            {
                MessageBox.Show("Korisnik sa datim JMBG već postoji.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var user = new User(0, jmbg, email, password, name, surname, phone, UserRole.Resident);

            try
            {
                long newId = userRepo.Create(user);
                MessageBox.Show("Registracija uspešna. ID: " + newId, "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri čuvanju korisnika: " + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}