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

        private void BtnEnd_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnEnd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
        }
    }
}
