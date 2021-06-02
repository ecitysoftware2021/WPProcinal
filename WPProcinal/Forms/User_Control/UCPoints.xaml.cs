using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPProcinal.Classes;
using WPProcinal.Service;

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
            txtPuntos.Text = $"¡En esta compra acumulaste {DataService41._DataResolution.DatosCliente.Cashback_Compra.ToString("C")} Caskback!";
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
