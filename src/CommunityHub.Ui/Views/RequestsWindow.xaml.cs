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
    public partial class RequestsWindow : Window
    {
        private readonly IBuildingAccessRequestService _requestService;
        private readonly IMapper<ResidentRequest, ResidentRequestDTO> _requestMapper;
        private readonly long _userId;

        public RequestsWindow(long userId)
        {
            _userId = userId;
            _requestService = Injector.CreateInstance<IBuildingAccessRequestService>();
            _requestMapper = Injector.CreateInstance<IMapper<ResidentRequest, ResidentRequestDTO>>();
            InitializeComponent();
            UcitajZahteve();
        }

        private void UcitajZahteve()
        {
            if (_requestService == null || _requestMapper == null || cmbFilter == null || dgRequests == null)
            {
                return;
            }

            RequestStatus? statusFilter = null;
            if (cmbFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                var content = selectedItem.Content?.ToString();
                if (content == "Na čekanju") statusFilter = RequestStatus.Pending;
                else if (content == "Odobreni") statusFilter = RequestStatus.Approved;
                else if (content == "Odbijeni") statusFilter = RequestStatus.Rejected;
            }

            var requests = _requestService.GetResidentRequests(_userId, statusFilter);
            dgRequests.ItemsSource = requests.Select(_requestMapper.Map).ToList();
        }

        private void CmbFilter_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UcitajZahteve();
        }

        private void BtnPovuciZahtev_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem is not ResidentRequestDTO selectedRequest)
            {
                MessageBox.Show("Morate odabrati zahtev.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedRequest.Status != RequestStatus.Pending.ToString())
            {
                MessageBox.Show("Možete povući samo zahteve koji su na čekanju.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Da li ste sigurni da želite da povučete ovaj zahtev?", "Potvrda",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                _requestService.CancelRequest(selectedRequest.Id, _userId);
                MessageBox.Show("Zahtev je uspešno povučen.", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                UcitajZahteve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}