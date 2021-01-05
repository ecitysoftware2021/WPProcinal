using System.Windows;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for FrmLoading.xaml
    /// </summary>

    public partial class FrmLoading : Window
    {

        public FrmLoading()
        {
            InitializeComponent();
            LblMessage.Text = string.Empty;
        }

        public FrmLoading(string message)
        {
            InitializeComponent();
            LblMessage.Text = message;
        }
    }
}
