using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application;
using CommunityHub.Application.Domain;
using CommunityHub.Application.DTO;
using CommunityHub.Application.DTO.DTOMappers;
using CommunityHub.Application.Service;

namespace CommunityHub.Ui.Views
{
    public partial class ManagerRequestsWindow : Window
    {
        private readonly IManagerService _managerService;
        private readonly IUserService _userService;
        private readonly IMapper<ResidentRequest, ResidentRequestDTO> _requestMapper;
        private readonly long _buildingId;
        private readonly long _managerId;

        public ManagerRequestsWindow(long buildingId, long managerId)
        {
            _buildingId = buildingId;
            _managerId = managerId;
            _managerService = Injector.CreateInstance<IManagerService>();
            _userService = Injector.CreateInstance<IUserService>();
            _requestMapper = Injector.CreateInstance<IMapper<ResidentRequest, ResidentRequestDTO>>();
            InitializeComponent();
            UcitajZahteve();
        }

        private void UcitajZahteve()
        {
            if (_managerService == null || _userService == null || _requestMapper == null || cmbFilter == null || dgRequests == null)
            {
                return;
            }

            RequestStatus? statusFilter = null;
            if (cmbFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                var content = selectedItem.Content?.ToString();
                if (content == "Na čekanju") statusFilter = RequestStatus.Pending;
                else if (content == "Odobreni") statusFilter = RequestStatus.Approved;
            }

            var requests = _managerService.GetRequestsForBuilding(_buildingId, statusFilter);

            if (statusFilter == null)
            {
                requests = requests.Where(r => r.Status != RequestStatus.Rejected).ToList();
            }

            dgRequests.ItemsSource = requests.Select(r =>
            {
                var dto = _requestMapper.Map(r);
                var user = _userService.GetById(r.UserId);
                dto.UserFullName = user != null ? $"{user.Name} {user.Surname}" : $"Korisnik #{r.UserId}";
                return dto;
            }).ToList();
        }

        private void CmbFilter_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UcitajZahteve();
        }

        private void BtnOdobriZahtev_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem is not ResidentRequestDTO selectedRequest)
            {
                MessageBox.Show("Morate odabrati zahtev.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedRequest.Status != RequestStatus.Pending.ToString())
            {
                MessageBox.Show("Možete odobriti samo zahteve koji su na čekanju.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Da li ste sigurni da želite da odobrite ovaj zahtev?", "Potvrda",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                _managerService.ApproveRequest(selectedRequest.Id, _managerId);
                MessageBox.Show("Zahtev je uspešno odobren!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                UcitajZahteve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOdbijZahtev_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem is not ResidentRequestDTO selectedRequest)
            {
                MessageBox.Show("Morate odabrati zahtev.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedRequest.Status != RequestStatus.Pending.ToString())
            {
                MessageBox.Show("Možete odbiti samo zahteve koji su na čekanju.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var reasonWin = new RejectReasonWindow();
            var dialogResult = reasonWin.ShowDialog();
            if (dialogResult != true || string.IsNullOrEmpty(reasonWin.RejectionReason)) return;

            try
            {
                _managerService.RejectRequest(selectedRequest.Id, _managerId, reasonWin.RejectionReason);
                MessageBox.Show("Zahtev je uspešno odbijen!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                UcitajZahteve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}