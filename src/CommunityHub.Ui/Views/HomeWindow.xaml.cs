using System;
using System.Windows;
using CommunityHub.Application.Domain;

namespace CommunityHub.Ui.Views
{
    public partial class HomeWindow : Window
    {
        private readonly long _userId;
        private readonly UserRole _userRole;

        public HomeWindow(long userId, UserRole userRole)
        {
            InitializeComponent();
            _userId = userId;
            _userRole = userRole;

            txtUloga.Text = $"Ulogovani ste kao: {(_userRole == UserRole.Administrator ? "Administrator" : _userRole == UserRole.Manager ? "Upravnik" : "Stanar")}";
        }

        private void BtnPregledZgrada_Click(object sender, RoutedEventArgs e)
        {
           
            BuildingSearchWindow buildingWin = new BuildingSearchWindow();
            buildingWin.ShowDialog();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
           
            LogInWindow loginWindow = new LogInWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}