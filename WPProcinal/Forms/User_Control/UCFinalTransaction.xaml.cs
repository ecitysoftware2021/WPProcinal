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
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Utilities.GetConfiguration("CashPayState").Equals("1"))
            {
                Utilities.ReValidatePayPad();
                Thread.Sleep(2000);
                Utilities.GoToInicial();
            }
            else
            {
                Thread.Sleep(2000);
                Utilities.GoToInicial();
            }
        }
    }
}
