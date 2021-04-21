using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPProcinal.Classes;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCPoints.xaml
    /// </summary>
    public partial class UCPoints : UserControl
    {
        public UCPoints()
        {
            InitializeComponent();
            //Dispatcher.BeginInvoke((Action)delegate
            //{
            txtPuntos.Text = $"¡En esta compra acumulaste {Math.Floor(Utilities.dataTransaction.PayVal / 1000)} puntos!";
            //});
            Utilities.Speack("Gracias por tu compra, retira tus boletas.");
            Utilities.dataTransaction = new DataTransaction();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                AdminPaypad.UpdatePeripherals();
                Thread.Sleep(3000);
                Dispatcher.BeginInvoke((Action)delegate
                {

                    Utilities.GoToInicial();
                });
            });
        }

    }
}
