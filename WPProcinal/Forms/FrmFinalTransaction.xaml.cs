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
            Task.Run(() =>
            {
                Thread.Sleep(5000);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Utilities.GoToInicial(this);
                });
            });
        }

        private void BtnConsult_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Utilities.GoToInicial(this);
        }
    }
}
