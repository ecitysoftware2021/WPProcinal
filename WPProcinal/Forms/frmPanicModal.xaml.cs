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
    /// Interaction logic for frmPanicModal.xaml
    /// </summary>
    public partial class frmPanicModal : Window
    {
        public frmPanicModal()
        {
            InitializeComponent();
        }

        private void BtnEnd_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Utilities.RestartApp();
        }
    }
}
