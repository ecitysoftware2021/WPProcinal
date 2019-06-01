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
    /// Interaction logic for Opciones.xaml
    /// </summary>
    public partial class Opciones : Window
    {
        string _peticion = string.Empty;
        bool _isCredit = false;
        Utilities Util;
        public Opciones(string mensaje, int len = 0, string peticion = null, bool isCredit = false)
        {
            InitializeComponent();
            txtOpcion.Text = mensaje;
            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
            //    new Action(delegate { }));

            txtultimosDigitos.MaxLength = len;
            Util = new Utilities();
            _peticion = peticion;
            _isCredit = isCredit;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isCredit)
            {
                if (_peticion != null)
                {
                    var respuestaPeticion = Util.EnviarPeticion(_peticion);
                    Utilities.CallBackRespuesta?.Invoke(respuestaPeticion);
                }
                else
                {
                    var respuestaPeticion = Util.EnviarPeticionEspera();
                    Utilities.CallBackRespuesta?.Invoke(respuestaPeticion);
                }
                Close();
            }
            else
            {
                gridTeclado.Visibility = Visibility.Visible;
            }
        }

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var boton = sender as TextBlock;
            if (boton.Tag.Equals("<"))
            {
                if (txtultimosDigitos.Text.Length > 0)
                {
                    txtultimosDigitos.Text = txtultimosDigitos.Text.Remove(txtultimosDigitos.Text.Length - 1, 1);
                }
            }
            else
            {
                if (txtultimosDigitos.Text.Length < txtultimosDigitos.MaxLength)
                {
                    txtultimosDigitos.Text += boton.Tag;
                }
            }

            if (txtultimosDigitos.Text.Length != txtultimosDigitos.MaxLength)
            {
                botonAceptar.Visibility = Visibility.Hidden;
            }
            else
            {
                botonAceptar.Visibility = Visibility.Visible;
            }
        }

        private void BotonAceptar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!txtultimosDigitos.Text.Equals("0"))
            {
                string peticion = Util.CalculateLRC(string.Concat(_peticion, txtultimosDigitos.Text, "]"));
                var respuestaPeticion = Util.EnviarPeticion(peticion);
                Utilities.CallBackRespuesta?.Invoke(respuestaPeticion);
                Close();
            }
            else
            {
                txtultimosDigitos.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

    }
}
