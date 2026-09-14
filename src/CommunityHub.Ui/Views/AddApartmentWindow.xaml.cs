using System;
using System.Windows;
using CommunityHub.Application;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Service;

namespace CommunityHub.Ui.Views
{
    public partial class AddApartmentWindow : Window
    {
        private readonly IManagerService _managerService;
        private readonly long _buildingId;

        public AddApartmentWindow(long buildingId)
        {
            _buildingId = buildingId;
            _managerService = Injector.CreateInstance<IManagerService>();
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtApartmentNumber.Text.Trim(), out int apartmentNumber))
            {
                MessageBox.Show("Broj stana mora biti brojčana vrijednost.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtNumberOfRooms.Text.Trim(), out int numberOfRooms))
            {
                MessageBox.Show("Broj soba mora biti brojčana vrijednost.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtMaxResidents.Text.Trim(), out int maxResidents))
            {
                MessageBox.Show("Maksimalan broj stanovnika mora biti brojčana vrijednost.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var apartment = new Apartment(0, apartmentNumber,
                    $"Stan broj {apartmentNumber}", numberOfRooms, maxResidents, _buildingId);

                _managerService.AddApartmentWithValidation(apartment);

                MessageBox.Show("Stan je uspešno dodat!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
