using System;
using System.Configuration;
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
            string reiniciarAplicacion = ConfigurationManager.AppSettings["ReinicioAplicacion"];
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    //if (Utilities.IsRestart)
                    //{
                    //    Utilities.RestartApp();
                    //}
                    //else
                    //{
                    //    if (Utilities.CantidadTransacciones >= 20 && reiniciarAplicacion.Equals("true"))
                    //    {
                    //        Utilities.RestartApp();
                    //    }
                    //    else
                    //    {
                    Utilities.GoToInicial(this);
                    //    }
                    //}
                });
            });
        }

        private void BtnConsult_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
