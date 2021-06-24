using Newtonsoft.Json;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WPProcinal.Classes;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCFinalTransaction.xaml
    /// </summary>
    public partial class UCFinalTransaction : UserControl
    {
        public UCFinalTransaction()
        {
            InitializeComponent();
            Utilities.Speack("Gracias por tu compra, retira tus boletas.");
            Utilities.dataTransaction = new DataTransaction();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Utilities.dataPaypad.PaypadConfiguration.enablE_VALIDATE_PERIPHERALS)
                {
                    await AdminPaypad.UpdatePeripherals();
                    Thread.Sleep(2000);
                    Utilities.GoToInicial();
                }
                else
                {
                    Thread.Sleep(2000);
                    Utilities.GoToInicial();
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCFinalTransaction>UserControl_Loaded", JsonConvert.SerializeObject(ex), 1);
                Utilities.GoToInicial();
            }
        }
    }
}
