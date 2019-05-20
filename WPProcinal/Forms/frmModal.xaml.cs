using System.Windows;
using System.Windows.Input;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmModal.xaml
    /// </summary>
    public partial class frmModal : Window
    {
        public frmModal(string message)
        {
            InitializeComponent();
            LblMessage.Text = message;
        }        

        private void BtnEnd_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = true;
        }
    }
}
