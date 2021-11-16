using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for frmDispenser.xaml
    /// </summary>
    public partial class frmDispenser : Window
    {
        public frmDispenser()
        {
            InitializeComponent();
        }

        private void BtnDispenser_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            if (!string.IsNullOrEmpty(TxtValor.Text))
            {
                Dispenser(Convert.ToDecimal(TxtValor.Text));
            }
        }

        private void Dispenser(decimal Valor)
        {
            if (Valor > 0)
            {
                //TODO: Mirar por que explota :,c
                Utilities.control.StartDispenser(Valor);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
