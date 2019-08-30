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


        private void BtnYes_TouchDown(object sender, TouchEventArgs e)
        {
            BtnYes.IsEnabled = false;
            Utilities.MedioPago = 1;
            DialogResult = true;
        }

        private void BtnNo_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnCard_TouchDown(object sender, TouchEventArgs e)
        {
            BtnCard.IsEnabled = false;
            Utilities.MedioPago = 2;
            DialogResult = true;
        }
    }
}
