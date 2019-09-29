using Grabador.Transaccion;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPProcinal.Classes;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for FrmFinalTransaction.xaml
    /// </summary>
    public partial class FrmFinalTransaction : Window
    {
        public FrmFinalTransaction()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    //Utilities.GoToInicial(this);
                });
            });
        }
    }
}
