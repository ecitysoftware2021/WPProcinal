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
    /// Interaction logic for WPlateModal.xaml
    /// </summary>
    public partial class WPlateModal : Window
    {
        public WPlateModal()
        {
            InitializeComponent();
        }

        private void BtnContinue_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.PLACA = txPlaca.Text.Trim();
            DialogResult = true;
        }

        private void Label_TouchDown(object sender, TouchEventArgs e)
        {
            var text = (sender as Label).Tag;
            if (text != "<")
            {
                txPlaca.Text += text;
            }
            else
            {
                if (txPlaca.Text.Length > 0)
                {
                    txPlaca.Text = txPlaca.Text.Substring(0, txPlaca.Text.Length - 1);
                }
            }
        }
    }
}
