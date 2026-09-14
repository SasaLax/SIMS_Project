using System;
using System.Windows;
using CommunityHub.Application;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Service;

namespace CommunityHub.Ui.Views
{
    public partial class AddBuildingWindow : Window
    {
        private readonly IAdminService _adminService;

        public AddBuildingWindow()
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
                if (string.IsNullOrWhiteSpace(txtBuildingCode.Text))
                {
                    MessageBox.Show("Unesite kod zgrade!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtUlica.Text))
                {
                    MessageBox.Show("Unesite ulicu!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtBroj.Text.Trim(), out int broj))
                {
                    MessageBox.Show("Broj mora biti brojčana vrednost!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNaselje.Text))
                {
                    MessageBox.Show("Unesite naselje!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtGrad.Text))
                {
                    MessageBox.Show("Unesite grad!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDrzava.Text))
                {
                    MessageBox.Show("Unesite državu!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtSpratovi.Text.Trim(), out int spratovi))
                {
                    MessageBox.Show("Broj spratova mora biti brojčana vrednost!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kreiramo objekat sa tačnim redoslijedom i tipovima iz Building konstruktora
                var building = new Building(
                    id: 0, // 0 jer baza sama generiše ID (auto-increment)
                    buildingCode: txtBuildingCode.Text.Trim(),
                    address: new Address(txtUlica.Text.Trim(), broj),
                    neighbourhood: txtNaselje.Text.Trim(),
                    location: new Location(txtGrad.Text.Trim(), txtDrzava.Text.Trim()),
                    numberOfFloors: spratovi,
                    managerJmbg: string.IsNullOrWhiteSpace(txtUpravnikJmbg.Text) ? null : txtUpravnikJmbg.Text.Trim(),
                    status: BuildingStatus.Pending
                );

                _adminService.AddBuilding(building);

                MessageBox.Show("Zgrada uspešno dodata! Upravnik treba da je odobri!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}