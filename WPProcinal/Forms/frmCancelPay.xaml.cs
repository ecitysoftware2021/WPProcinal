using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmCancelPay.xaml
    /// </summary>
    public partial class frmCancelPay : Window
    {               
        Utilities objUtil = new Utilities();
        List<TypeSeat> typeSeatsCurrent = new List<TypeSeat>();
        DipMap dipMapCurrent = new DipMap();
        decimal _valorDevolver = 0;

        public frmCancelPay(decimal Valor, List<TypeSeat> typeSeats, DipMap dipMap)
        {
            
            InitializeComponent();
            _valorDevolver = Valor;
            lblValorDevolver.Text = string.Format("Valor a Devolver:  {0:C0}", Valor);
            typeSeatsCurrent = typeSeats;
            dipMapCurrent = dipMap;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var frmloading = new FrmLoading();
            frmloading.Show();
            Utilities.CancelAssing(typeSeatsCurrent, dipMapCurrent);
            frmloading.Close();
            StartProcess();
        }

        private async void StartProcess()
        {
            ActivatePayments activatePayments = new ActivatePayments();
            await activatePayments.Devolver();
            Print();
            //Utilities.GoToInicial();
        }

        private void Print()
        {
            CLSWCFPayPad objWCFPayPad = new CLSWCFPayPad();
            foreach (var item in Utilities.IDTransaccionDBs)
            {
                objWCFPayPad.UpdateTransactionCancel(item);
            }
            objUtil.ImprimirComprobanteCancelado("Rechazada", _valorDevolver, dipMapCurrent);
        }   
    }
}
