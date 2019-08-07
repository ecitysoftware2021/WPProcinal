using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        TPVOperation TPV;
        DataCardTransaction _dataCard;
        public Opciones(DataCardTransaction dataCard)
        {
            InitializeComponent();
            _dataCard = dataCard;
            //txtOpcion.Text = mensaje;
            //txtultimosDigitos.MaxLength = maxlen;
            //txtultimosDigitos.MinLines = minlen;
            TPV = new TPVOperation();
            _peticion = dataCard.peticion;
            _isCredit = dataCard.isCredit;
            gridOperaciones.DataContext = _dataCard;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isCredit)
            {
                Task.Run(() =>
                {
                    if (_peticion != null)
                    {

                        var respuestaPeticion = TPV.EnviarPeticion(_peticion);
                        TPVOperation.CallBackRespuesta?.Invoke(respuestaPeticion);
                    }
                    else
                    {
                        var respuestaPeticion = TPV.EnviarPeticionEspera();
                        TPVOperation.CallBackRespuesta?.Invoke(respuestaPeticion);
                    }
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Close();
                    });
                });

            }
            else
            {
                gridTeclado.Visibility = Visibility.Visible;
            }
        }

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //NumbersButtons(sender);
        }

        private void NumbersButtons(object sender)
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

            if (txtultimosDigitos.Text.Length != txtultimosDigitos.MaxLength
                && txtultimosDigitos.Text.Length < txtultimosDigitos.MinLines)
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
            //AcceptButton();
        }

        private void AcceptButton()
        {

            if (!txtultimosDigitos.Text.Equals("0"))
            {
                FrmLoading frmLoading = new FrmLoading("Esperando confirmación del pago...");
                if (txtultimosDigitos.Text.Length <= 2)
                {
                    this.Hide();
                    frmLoading.Show();
                    TPVOperation.Quotas = int.Parse(txtultimosDigitos.Text);
                }
                string peticion = TPV.CalculateLRC(string.Concat(_peticion, txtultimosDigitos.Text, "]"));
                var respuestaPeticion = TPV.EnviarPeticion(peticion);
                TPVOperation.CallBackRespuesta?.Invoke(respuestaPeticion);
                frmLoading.Close();
                Close();
            }
            else
            {
                txtultimosDigitos.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void TextBlock_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            NumbersButtons(sender);
        }

        private void BotonAceptar_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            _dataCard.mensaje = "Esperando datáfono...";
            _dataCard.visible = "Hidden";
            AcceptButton();
        }
    }
}
