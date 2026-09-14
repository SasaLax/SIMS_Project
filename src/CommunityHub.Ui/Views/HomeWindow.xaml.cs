using System;
using System.Windows;
using CommunityHub.Application.Domain;

namespace CommunityHub.Ui.Views
{
    public partial class HomeWindow : Window
    {
        private readonly long _userId;
        private readonly UserRole _userRole;
        private readonly string _userJmbg;

        public HomeWindow(long userId, UserRole userRole, string userJmbg)
        {
            InitializeComponent();
            _userId = userId;
            _userRole = userRole;
            _userJmbg = userJmbg;

            txtUloga.Text = $"Ulogovani ste kao: {(_userRole == UserRole.Administrator ? "Administrator" : _userRole == UserRole.Manager ? "Upravnik" : "Stanar")}";

            // Prikazujemo različita dugmad na osnovu uloge
            if (_userRole == UserRole.Resident)
            {
                btnPregledZgradaStanar.Visibility = Visibility.Visible;
                btnMojiZahtevi.Visibility = Visibility.Visible;
                btnMojeZgrade.Visibility = Visibility.Collapsed;
                btnDodajUpravnika.Visibility = Visibility.Collapsed;
                btnDodajZgradu.Visibility = Visibility.Collapsed;
            }
            else if (_userRole == UserRole.Manager)
            {
                btnPregledZgradaStanar.Visibility = Visibility.Visible;
                btnMojeZgrade.Visibility = Visibility.Visible;
                btnDodajUpravnika.Visibility = Visibility.Collapsed;
                btnDodajZgradu.Visibility = Visibility.Collapsed;
            }
            else if (_userRole == UserRole.Administrator)
            {
                btnPregledZgradaStanar.Visibility = Visibility.Visible;
                btnMojiZahtevi.Visibility = Visibility.Collapsed;
                btnMojeZgrade.Visibility = Visibility.Collapsed;
                btnDodajUpravnika.Visibility = Visibility.Visible;
                btnDodajZgradu.Visibility = Visibility.Visible;
            }
        }

        private void BtnPregledZgrada_Click(object sender, RoutedEventArgs e)
        {
            BuildingSearchWindow buildingWin = new BuildingSearchWindow(_userId, _userRole);
            buildingWin.ShowDialog();
        }

        private void BtnMojiZahtevi_Click(object sender, RoutedEventArgs e)
        {
            RequestsWindow requestsWin = new RequestsWindow(_userId);
            requestsWin.ShowDialog();
        }

        private void BtnMojeZgrade_Click(object sender, RoutedEventArgs e)
        {
            var managerBuildingsWindow = new ManagerBuildingsWindow(_userJmbg, _userId);
            managerBuildingsWindow.ShowDialog();
        }

        private void BtnZahtevi_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ova funkcionalnost je u pripremi!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDodajUpravnika_Click(object sender, RoutedEventArgs e)
        {
            var addManagerWindow = new AddManagerWindow();
            addManagerWindow.ShowDialog();
        }

        private void BtnDodajZgradu_Click(object sender, RoutedEventArgs e)
        {
            var addBuildingWindow = new AddBuildingWindow();
            addBuildingWindow.ShowDialog();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LogInWindow loginWindow = new LogInWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}