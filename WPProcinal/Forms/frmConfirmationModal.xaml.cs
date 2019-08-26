using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPProcinal.Classes;
using WPProcinal.Models;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmConfirmationModal.xaml
    /// </summary>
    public partial class frmConfirmationModal : Window
    {
        int payCardState = int.Parse(Utilities.GetConfiguration("CardPayState"));
        public frmConfirmationModal(List<TypeSeat> typeSeats, DipMap dipMap)
        {
            InitializeComponent();
            BtnCard.Visibility = payCardState == 1 ? Visibility.Visible : Visibility.Hidden;
            decimal totalModal = 0;
            decimal totalPago = 0;
            foreach (var seat in typeSeats)
            {
                totalPago += seat.Price;
                seat.Price = Utilities.RoundValue(seat.Price);
                totalModal += seat.Price;
            }

            TxtTitle.Text = Utilities.CapitalizeFirstLetter(dipMap.MovieName);
            TxtRoom.Text = dipMap.RoomName;
            TxtDate.Text = string.Format("{0} {1}", dipMap.Day, dipMap.HourFunction);
            TxtTotal.Text = string.Format("{0:C0}", totalModal);
            lvListSeats.ItemsSource = typeSeats.OrderBy(s => s.Name);
            Utilities.ValorPagarScore = totalPago;
            Utilities.PayVal = totalModal;
        }

        private void BtnYes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BtnYes.IsEnabled = false;
            Utilities.MedioPago = 1;
            DialogResult = true;
        }

        private void BtnNo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnYes_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            BtnYes.IsEnabled = false;
            Utilities.MedioPago = 1;
            DialogResult = true;
        }

        private void BtnNo_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnYes_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BtnYes.IsEnabled = false;
            Utilities.MedioPago = 1;
            DialogResult = true;
        }

        private void BtnNo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnCard_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            BtnCard.IsEnabled = false;
            Utilities.MedioPago = 2;
            DialogResult = true;
        }

        private void BtnCard_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BtnCard.IsEnabled = false;
            Utilities.MedioPago = 2;
            DialogResult = true;
        }
    }
}
