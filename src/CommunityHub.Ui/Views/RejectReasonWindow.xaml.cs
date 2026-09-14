using System.Windows;

namespace CommunityHub.Ui.Views
{
    public partial class RejectReasonWindow : Window
    {
        public string RejectionReason { get; private set; } = string.Empty;

        public RejectReasonWindow()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReason.Text))
            {
                MessageBox.Show("Morate uneti razlog odbijanja.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            RejectionReason = txtReason.Text;
            DialogResult = true;
            Close();
        }
    }
}