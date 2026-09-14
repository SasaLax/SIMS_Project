using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommunityHub.Application;
using CommunityHub.Application.Domain;
using CommunityHub.Application.DTO;
using CommunityHub.Application.DTO.DTOMappers;
using CommunityHub.Application.Service;
using CommunityHub.Ui.Helpers;

namespace CommunityHub.Ui.Views
{
    public partial class BuildingSearchWindow : Window
    {
        private readonly IBuildingService _buildingService;
        private readonly IMapper<Building, BuildingDTO> _buildingMapper;
        private readonly long _userId;
        private readonly UserRole _userRole;

        public BuildingSearchWindow(long userId, UserRole userRole)
        {
            _userId = userId;
            _userRole = userRole;
            _buildingService = Injector.CreateInstance<IBuildingService>();
            _buildingMapper = Injector.CreateInstance<IMapper<Building, BuildingDTO>>();
            InitializeComponent();

            if (_userRole != UserRole.Resident)
            {
                btnPodnesiZahtev.Visibility = Visibility.Collapsed;
            }

            UcitajSveZgrade();
        }

        private void UcitajSveZgrade()
        {
            if (_buildingService == null || _buildingMapper == null || dgBuildings == null || chkSortFloors == null)
            {
                return;
            }

            try
            {
                bool sort = chkSortFloors.IsChecked ?? false;
                var buildings = _buildingService.GetAll(sort, onlyApproved: true);
                dgBuildings.ItemsSource = buildings.Select(_buildingMapper.Map).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju zgrada:\n{ex.Message}",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Txt_TextChanged(object sender, RoutedEventArgs e)
        {
            if (_buildingService == null || _buildingMapper == null || dgBuildings == null)
            {
                return;
            }

            try
            {
                List<BuildingDTO> buildings;

                if (!string.IsNullOrEmpty(txtAddress.Text.Trim()))
                {
                    buildings = _buildingService.SearchByAddress(txtAddress.Text.Trim())
                        .Select(_buildingMapper.Map).ToList();
                }
                else if (!string.IsNullOrEmpty(txtNeighbourhood.Text.Trim()))
                {
                    buildings = _buildingService.SearchByNeighbourhood(txtNeighbourhood.Text.Trim())
                        .Select(_buildingMapper.Map).ToList();
                }
                else if (!string.IsNullOrEmpty(txtFloors.Text.Trim()))
                {
                    if (!int.TryParse(txtFloors.Text.Trim(), out int spratovi))
                    {
                        dgBuildings.ItemsSource = new List<BuildingDTO>();
                        return;
                    }

                    buildings = _buildingService.SearchByFloors(spratovi)
                        .Select(_buildingMapper.Map).ToList();
                }
                else if (!string.IsNullOrEmpty(txtApartments.Text.Trim()))
                {
                    buildings = BuildingSearchParser.ParseAndSearch(txtApartments.Text.Trim())
                        .Select(_buildingMapper.Map).ToList();
                }
                else
                {
                    UcitajSveZgrade();
                    return;
                }

                if (chkSortFloors.IsChecked == true)
                {
                    buildings = buildings.OrderBy(b => b.NumberOfFloors).ToList();
                }

                dgBuildings.ItemsSource = buildings;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška pri pretrazi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ChkSortFloors_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            Txt_TextChanged(sender, e);
        }

        private void BtnPodnesiZahtev_Click(object sender, RoutedEventArgs e)
        {
            if (dgBuildings.SelectedItem is not BuildingDTO selectedBuilding)
            {
                MessageBox.Show("Morate odabrati zgradu.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var createRequestWindow = new CreateRequestWindow(selectedBuilding, _userId);
            createRequestWindow.ShowDialog();
        }
    }
}
