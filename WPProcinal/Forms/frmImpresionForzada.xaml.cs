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
    /// Interaction logic for frmImpresionForzada.xaml
    /// </summary>
    public partial class frmImpresionForzada : Window
    {
        public frmImpresionForzada()
        {
            InitializeComponent();
        }

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var boton = sender as TextBlock;
            if (boton.Tag.Equals("<"))
            {
                if (txtIDTransaccion.Text.Length > 0)
                {
                    txtIDTransaccion.Text = txtIDTransaccion.Text.Remove(txtIDTransaccion.Text.Length - 1, 1);
                }
            }
            else
            {
                if (txtIDTransaccion.Text.Length < txtIDTransaccion.MaxLength)
                {
                    txtIDTransaccion.Text += boton.Tag;
                }
            }
        }

        private void BotonAceptar_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIDTransaccion.Text))
            {
                return;
            }
            Utilities.ImprimirComprobanteForzado(int.Parse(txtIDTransaccion.Text));
        }
    }
}
