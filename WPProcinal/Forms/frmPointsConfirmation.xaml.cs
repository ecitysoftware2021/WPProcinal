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

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmPointsConfirmation.xaml
    /// </summary>
    public partial class frmPointsConfirmation : Window
    {
        public frmPointsConfirmation()
        {
            InitializeComponent();
        }

        private void BtnNot_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnYes_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = true;
        }
    }
}
