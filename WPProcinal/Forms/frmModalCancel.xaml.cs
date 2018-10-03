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
    /// Interaction logic for frmModalCancel.xaml
    /// </summary>
    public partial class frmModalCancel : Window
    {
        public frmModalCancel(string message)
        {
            InitializeComponent();
            LblMessage.Text = message;
        }

        private void BtnConfirm_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnCancel_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            DialogResult = false;
        }
    }
}
