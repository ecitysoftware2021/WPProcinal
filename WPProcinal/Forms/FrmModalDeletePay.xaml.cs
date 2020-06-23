using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPProcinal.Classes;
using WPProcinal.Service;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for FrmModalDeletePay.xaml
    /// </summary>
    public partial class FrmModalDeletePay : Window
    {
        public FrmModalDeletePay()
        {
            InitializeComponent();
        }

        private void BtSalir_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = true;
        }

        private async void BtIngresar_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                int secuencia = Convert.ToInt32(txtSecuencia.Text);

                var response = WCFServices41.CancelSale(new SCORET
                {
                    Punto = Convert.ToInt32(Utilities.GetConfiguration("Cinema")),
                    Pedido = secuencia,
                    teatro = Utilities.GetConfiguration("CodCinema"),
                    tercero = "1"
                });
                if (response != null)
                {
                    if (response[0].Respuesta != null)
                    {
                        if (response[0].Respuesta.ToLower().Contains("proceso exitoso"))
                        {
                            txtMs.Text = "Compra eliminada";

                            Task.Run(() =>
                            {
                                Thread.Sleep(2000);
                                Dispatcher.BeginInvoke((Action)delegate
                                {
                                    DialogResult = true;
                                });
                            });
                        }
                    }
                    else
                    {
                        txtMs.Text = response[0].Validacion;
                    }
                }
                else
                {
                    txtMs.Text = "No se pudo eliminar la factura.";
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
