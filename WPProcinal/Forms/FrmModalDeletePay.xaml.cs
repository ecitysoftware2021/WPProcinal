using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
                int secuenca = Convert.ToInt32(txtSecuencia.Text);

                int num = WCFServices41.CancelSale(secuenca);

                if (num == 1)
                {
                    txtMs.Text = "Compra eliminada";

                    Task.Run(() =>
                    {
                        Thread.Sleep(3000);
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            DialogResult = true;
                        });
                    });
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
