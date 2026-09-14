using System;
using System.Windows;
using CommunityHub.Application;
using CommunityHub.Application.DTO;
using CommunityHub.Application.Service;

namespace CommunityHub.Ui.Views
{
    public partial class CreateRequestWindow : Window
    {
        private readonly IBuildingAccessRequestService _requestService;
        private readonly IBuildingService _buildingService;
        private readonly BuildingDTO _selectedBuilding;
        private readonly long _userId;
        private bool _occupancyWarningShown = false;

        public CreateRequestWindow(BuildingDTO building, long userId)
        {
            InitializeComponent();
            _requestService = Injector.CreateInstance<IBuildingAccessRequestService>();
            _buildingService = Injector.CreateInstance<IBuildingService>();
            _selectedBuilding = building;
            _userId = userId;

            txtBuilding.Text = $"{building.BuildingCode} - {building.FullAddress}";
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtApartmentNumber.Text.Trim(), out int apartmentNumber))
            {
                MessageBox.Show("Broj stana mora biti brojčana vrijednost.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_buildingService.ExistsApartmentNumberInBuilding(apartmentNumber, _selectedBuilding.Id))
            {
                MessageBox.Show("Stan sa unetim brojem ne postoji u odabranoj zgradi.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (_requestService.IsApartmentAlreadyOccupied(_selectedBuilding.Id, apartmentNumber) && !_occupancyWarningShown)
                {
                    pnlWarning.Visibility = Visibility.Visible;
                    txtWarning.Text = "Već postoji stanar za ovaj broj stana! Proverite da li ste uneli ispravan broj. Ako jeste, ponovo kliknite \"Podnesi zahtev\" da potvrdite.";
                    _occupancyWarningShown = true;
                    return;
                }

                _requestService.CreateRequest(_userId, _selectedBuilding.Id, apartmentNumber);

                MessageBox.Show("Zahtev je uspešno podnesen!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
