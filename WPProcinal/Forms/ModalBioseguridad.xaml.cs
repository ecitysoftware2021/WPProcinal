using System.Windows;
using System.Windows.Input;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for ModalBioseguridad.xaml
    /// </summary>
    public partial class ModalBioseguridad : Window
    {
        public ModalBioseguridad()
        {
            InitializeComponent();
        }

        private void Image_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = true;
        }
    }
}
