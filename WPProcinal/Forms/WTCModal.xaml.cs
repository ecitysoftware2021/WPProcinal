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
    /// Interaction logic for WTCModal.xaml
    /// </summary>
    public partial class WTCModal : Window
    {
        public WTCModal()
        {
            InitializeComponent();
            LblMessage.Text = string.Format(Utilities.GetConfiguration("MensajeCinefans"), Environment.NewLine + Environment.NewLine);
            LblURL.Text = Utilities.GetConfiguration("MensajeURL");
        }

        private void BtnEnd_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = true;
        }
    }
}
