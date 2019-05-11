using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;
using System.Globalization;
using CEntidades;
using Newtonsoft.Json;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmPay.xaml
    /// </summary>
    public partial class frmPay : Window
    {
        #region Referencias                
        PayViewModel PayViewModel;
        ActivatePayments activatePayments;
        #endregion

        public frmPay()
        {
            InitializeComponent();
        }

        public frmPay(List<TypeSeat> Seats, DipMap dipMap)
        {
            InitializeComponent();
            //activatePayments = new ActivatePayments();
            //PayViewModel = new PayViewModel
            //{
            //    ValorFaltante = "$ 0,00",
            //    ValorIngresado = "$ 0,00",
            //    ValorRestante = "$ 0,00",
            //    ImgCancel = Visibility.Visible,
            //    ImgEspereCambio = Visibility.Visible,
            //    ImgIngreseBillete = Visibility.Visible,
            //    ImgLeyendoBillete = Visibility.Visible,
            //    ImgRecibo = Visibility.Visible,                
            //};

            this.DataContext = PayViewModel;
            //Utilities.TypeSeats = Seats;
            //Utilities.DipMapCurrent = dipMap;
            //Utilities.DipMapCurrent.Total = double.Parse(Utilities.ValorPagar.ToString());            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Utilities.ValorPagar = 6000;
          // RoundValue();
            //PayViewModel.ValorFaltante = Utilities.ValorPagar.ToString("C", CultureInfo.CurrentCulture);
            //activatePayments.PayViewModel = PayViewModel;
            //activatePayments.HideImages();
            //activatePayments.TimerTime();
            //var t2 = Task.Run(() =>
            //{
            //    GetReceipt();
            //});

            //this.Dispatcher.Invoke(() =>
            //{
            //    lblValorPagar.Content = Utilities.ValorPagar.ToString("C", CultureInfo.CurrentCulture);                
            //});

            activatePayments.PagoEfectivo();
            activatePayments.PagoMonedero();            
        }

        //private void GetReceipt()
        //{
        //    var response = WCFServices.GetReceiptProcinal(ControlPantalla.IdCorrespo);
        //    if (response.IsSuccess)
        //    {
        //        Utilities.Receipt = JsonConvert.DeserializeObject<Receipt>(response.Result.ToString());
        //    }
        //}
     

        //private void RoundValue()
        //{
        //    decimal value = Utilities.ValorPagar;
        //    decimal mod = value % 100;
        //    mod = mod % 100;
        //    if (mod > 0)
        //    {
        //        Utilities.ValorPagar = value - mod;
        //    }
        //}


        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EndTransaction();
        }

        void EndTransaction()
        {
            activatePayments.EndTransactionCancel(this);
        }

        private void BtnCancel_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            EndTransaction();
        }
    }
}
