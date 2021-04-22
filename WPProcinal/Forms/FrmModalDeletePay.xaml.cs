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
                    Punto =Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.AMBIENTE.puntoVenta,
                    Pedido = secuencia,
                    teatro = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema.ToString(),
                    tercero = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.tercero
                });
                if (response != null)
                {
                    if (response[0].Respuesta != null)
                    {
                        if (response[0].Respuesta.ToLower().Contains("proceso exitoso"))
                        {
                            txtMs.Text = "Compra eliminada";

                            //TODO: Imprimir un comprobante y cambiar estado de la transaccion en la base de datos
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
