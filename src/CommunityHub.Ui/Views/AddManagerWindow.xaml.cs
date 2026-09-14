using System;
using System.Windows;
using CommunityHub.Application;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Service;

namespace CommunityHub.Ui.Views
{
    public partial class AddManagerWindow : Window
    {
        private readonly IAdminService _adminService;

        public AddManagerWindow()
        {
            InitializeComponent();
            _adminService = Injector.CreateInstance<IAdminService>();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtIme.Text))
                {
                    MessageBox.Show("Unesite ime!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtPrezime.Text))
                {
                    MessageBox.Show("Unesite prezime!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtJmbg.Text))
                {
                    MessageBox.Show("Unesite JMBG!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Unesite email!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtLozinka.Password))
                {
                    MessageBox.Show("Unesite lozinku!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                var manager = new User(
                    id: 0,
                    jmbg: txtJmbg.Text.Trim(),
                    email: txtEmail.Text.Trim(),
                    password: txtLozinka.Password,
                    name: txtIme.Text.Trim(),
                    surname: txtPrezime.Text.Trim(),
                    phoneNumber: txtTelefon.Text.Trim(),
                    role: UserRole.Manager);
                
                _adminService.RegisterManager(manager);
                
                MessageBox.Show("Upravnik uspešno dodat!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}