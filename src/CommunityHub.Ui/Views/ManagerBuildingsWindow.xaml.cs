using CommunityHub.Application;
using CommunityHub.Application.Domain;
using CommunityHub.Application.DTO;
using CommunityHub.Application.DTO.DTOMappers;
using CommunityHub.Application.Service;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views
{
    public partial class ManagerBuildingsWindow : Window
    {
        private readonly IManagerService _managerService;
        private readonly IMapper<Building, BuildingDTO> _buildingMapper;
        private readonly string _managerJmbg;
        private readonly long _managerId;

        public ManagerBuildingsWindow(string managerJmbg, long managerId)
        {
            _managerJmbg = managerJmbg;
            _managerId = managerId;
            _managerService = Injector.CreateInstance<IManagerService>();
            _buildingMapper = Injector.CreateInstance<IMapper<Building, BuildingDTO>>();

            InitializeComponent();
            UcitajZgrade();
        }

        private void UcitajZgrade()
        {
            if (_managerService == null || _buildingMapper == null || cmbFilter == null || dgBuildings == null)
            {
                return;
            }

            string? statusFilter = null;
            if (cmbFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                var content = selectedItem.Content?.ToString();
                if (content == "Na čekanju") statusFilter = "Pending";
                else if (content == "Odobrene") statusFilter = "Approved";
            }

            var buildings = _managerService.GetMyBuildings(_managerJmbg);

            if (!string.IsNullOrEmpty(statusFilter))
            {
                buildings = buildings.Where(b => b.Status.ToString() == statusFilter).ToList();
            }

            dgBuildings.ItemsSource = buildings.Select(_buildingMapper.Map).ToList();
        }

        private void CmbFilter_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UcitajZgrade();
        }

        private void BtnOdobriZgradu_Click(object sender, RoutedEventArgs e)
        {
            if (dgBuildings.SelectedItem is not BuildingDTO selectedBuilding)
            {
                MessageBox.Show("Morate odabrati zgradu.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Da li ste sigurni da želite da odobrite ovu zgradu?", "Potvrda",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                _managerService.ApproveBuilding(selectedBuilding.Id);

                MessageBox.Show("Zgrada je uspešno odobrena!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                UcitajZgrade();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOdbijZgradu_Click(object sender, RoutedEventArgs e)
        {
            if (dgBuildings.SelectedItem is not BuildingDTO selectedBuilding)
            {
                MessageBox.Show("Morate odabrati zgradu.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Da li ste sigurni da želite da odbijete ovu zgradu?", "Potvrda",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                _managerService.RejectBuilding(selectedBuilding.Id);

                MessageBox.Show("Zgrada je uspešno odbijena!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                UcitajZgrade();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPogledajZahteve_Click(object sender, RoutedEventArgs e)
        {
            if (dgBuildings.SelectedItem is not BuildingDTO selectedBuilding)
            {
                MessageBox.Show("Morate odabrati zgradu.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedBuilding.Status != "Approved")
            {
                MessageBox.Show("Možete pogledati zahteve samo za odobrene zgrade.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var requestsWin = new ManagerRequestsWindow(selectedBuilding.Id, _managerId);
            requestsWin.ShowDialog();
        }

        private void BtnDodajStan_Click(object sender, RoutedEventArgs e)
        {
            if (dgBuildings.SelectedItem is not BuildingDTO selectedBuilding)
            {
                MessageBox.Show("Morate odabrati zgradu.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addApartmentWin = new AddApartmentWindow(selectedBuilding.Id);
            addApartmentWin.ShowDialog();
        }
    }
}