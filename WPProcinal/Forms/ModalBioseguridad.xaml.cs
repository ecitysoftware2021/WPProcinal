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
