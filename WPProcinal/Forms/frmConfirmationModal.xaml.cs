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
        public frmConfirmationModal(List<TypeSeat> typeSeats, DipMap dipMap)
        {
            InitializeComponent();
            decimal total = 0;
            foreach (var seat in typeSeats)
            {
                total += seat.Price;
            }

            TxtTitle.Text = Utilities.CapitalizeFirstLetter(dipMap.MovieName);
            TxtRoom.Text = dipMap.RoomName;
            TxtDate.Text = string.Format("{0} {1}",dipMap.Day,dipMap.HourFunction);
            TxtTotal.Text = string.Format("{0:C0}", total);
            lvListSeats.ItemsSource = typeSeats.OrderBy(s => s.Name);
            Utilities.ValorPagar = total;
        }

        private void BtnYes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnNo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnYes_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnNo_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnYes_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
        }
    }
}
