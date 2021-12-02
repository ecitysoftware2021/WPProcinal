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
    /// Interaction logic for ModalCovid.xaml
    /// </summary>
    public partial class ModalCovid : Window
    {
        public ModalCovid()
        {
            InitializeComponent();
            mesajeCovid.Text = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.MensajeCovid;

   
        }

        private void BtnEnd_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = true;
        }
    }
}
