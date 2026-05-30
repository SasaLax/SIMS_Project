using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Ui.Helpers; // Obavezno proveri da li ti se namespace poklapa sa mestom gde je BuildingSearchParser

namespace CommunityHub.Ui.Views
{
    public partial class BuildingSearchWindow : Window
    {
        private readonly BuildingDbRepository _buildingRepository;

        public BuildingSearchWindow()
        {
            InitializeComponent();
            _buildingRepository = new BuildingDbRepository();

            // Čim se prozor otvori, odmah učitavamo sve zgrade iz baze
            UcitajZgrade();
        }

        /// <summary>
        /// Pomoćna metoda koja osvežava tabelu (DataGrid) sa svim zgradama, uzimajući u obzir i Sortiranje.
        /// </summary>
        private void UcitajZgrade()
        {
            try
            {
                bool sort = chkSortFloors.IsChecked ?? false;
                dgBuildings.ItemsSource = _buildingRepository.GetAll(sort);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju zgrada: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Klik na dugme "Traži". Detektuje koji je filter izabran i pokreće odgovarajući upit.
        /// </summary>
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string unesiText = txtSearchValue.Text.Trim();
            int selektovaniIndeks = cmbSearchType.SelectedIndex;

            if (selektovaniIndeks == 0)
            {
                UcitajZgrade();
                return;
            }

            if (string.IsNullOrEmpty(unosiText))
            {
                MessageBox.Show("Molimo unesite vrednost za pretragu.", "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                switch (selektovaniIndeks)
                {
                    case 1: // Pretraga po adresi (ulica i broj, neosetljivo na slova, radi i delimičan unos)
                        dgBuildings.ItemsSource = _buildingRepository.SearchByAddress(