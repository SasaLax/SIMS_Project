using System;
using System.Collections.Generic;
using System.Windows;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Ui.Helpers;

namespace CommunityHub.Ui.Views
{
    public partial class BuildingSearchWindow : Window
    {
        private readonly BuildingDbRepository _buildingRepository;

        public BuildingSearchWindow()
        {
            InitializeComponent();
            _buildingRepository = new BuildingDbRepository();

            UcitajSveZgrade();
        }

        private void UcitajSveZgrade()
        {
            bool sort = chkSortFloors.IsChecked ?? false;
            dgBuildings.ItemsSource = _buildingRepository.GetAll(sort);
        }


        private void BtnShowAll_Click(object sender, RoutedEventArgs e)
        {
            txtAddress.Text = "";
            txtNeighbourhood.Text = "";
            txtFloors.Text = "";
            txtApartments.Text = "";

            UcitajSveZgrade();
        }

     
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (!string.IsNullOrEmpty(txtAddress.Text.Trim()))
                {
                    dgBuildings.ItemsSource = _buildingRepository.SearchByAddress(txtAddress.Text.Trim());
                }
               
                else if (!string.IsNullOrEmpty(txtNeighbourhood.Text.Trim()))
                {
                    dgBuildings.ItemsSource = _buildingRepository.SearchByNeighbourhood(txtNeighbourhood.Text.Trim());
                }
               
                else if (!string.IsNullOrEmpty(txtFloors.Text.Trim()))
                {
                    if (int.TryParse(txtFloors.Text.Trim(), out int spratovi))
                    {
                        dgBuildings.ItemsSource = _buildingRepository.SearchByFloors(spratovi);
                    }
                    else
                    {
                        MessageBox.Show("Broj spratova mora biti brojčana vrijednost.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                
                else if (!string.IsNullOrEmpty(txtApartments.Text.Trim()))
                {
                    dgBuildings.ItemsSource = BuildingSearchParser.ParseAndSearch(txtApartments.Text.Trim());
                }
               
                else
                {
                    UcitajSveZgrade();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška pri pretrazi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

      
        private void ChkSortFloors_Changed(object sender, RoutedEventArgs e)
        {
           
            if (string.IsNullOrEmpty(txtAddress.Text) &&
                string.IsNullOrEmpty(txtNeighbourhood.Text) &&
                string.IsNullOrEmpty(txtFloors.Text) &&
                string.IsNullOrEmpty(txtApartments.Text))
            {
                UcitajSveZgrade();
            }
        }
    }
}