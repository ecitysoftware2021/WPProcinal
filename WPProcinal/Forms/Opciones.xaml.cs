using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
                        try
                        {
                            LogService.SaveRequestResponse(DateTime.Now + " :: Petición al datáfono: ", _peticion);
                        }
                        catch { }
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


        private void NumbersButtons(object sender)
        {
            var boton = sender as TextBlock;
            if (boton.Tag.Equals("<"))
            {
                if (txtultimosDigitos.Text.Length > 0)
                {
                    txtultimosDigitos.Tag = txtultimosDigitos.Tag.ToString().Remove(txtultimosDigitos.Tag.ToString().Length - 1, 1);
                    txtultimosDigitos.Text = txtultimosDigitos.Text.Remove(txtultimosDigitos.Text.Length - 1, 1);
                }
            }
            else
            {
                if (txtultimosDigitos.Text.Length < txtultimosDigitos.MaxLength)
                {
                    txtultimosDigitos.Tag += boton.Tag.ToString();
                    txtultimosDigitos.Text = "*".PadRight(txtultimosDigitos.Tag.ToString().Length, '*');
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

        private void AcceptButton()
        {

            if (!txtultimosDigitos.Text.Equals("0"))
            {
                FrmLoading frmLoading = new FrmLoading("Esperando confirmación del pago...");
                string data = ValidateDataMasc();
                if (txtultimosDigitos.Text.Length <= 2)
                {
                    this.Hide();
                    frmLoading.Show();
                    TPVOperation.Quotas = int.Parse(data);
                }

                string peticion = TPV.CalculateLRC(string.Concat(_peticion, data, "]"));
                try
                {
                    LogService.SaveRequestResponse(DateTime.Now + " :: Petición al datáfono: ", peticion);
                }
                catch { }
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

        string ValidateDataMasc()
        {
            string data;
            if (txtultimosDigitos.Text.Contains("*"))
            {
                data = txtultimosDigitos.Tag.ToString();
            }
            else
            {
                data = txtultimosDigitos.Text;
            }
            return data;
        }

        private ImageSource GetImage(string ckeck)
        {
            string icon = "img_show_hider";
            if (ckeck == "no")
            {
                icon = "img_show_hider";
            }
            else if (ckeck == "yes")
            {
                icon = "img_show";
            }

            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(string.Concat("/Images/Buttons/", icon, ".png"), UriKind.Relative);
            logo.EndInit();
            return logo;
        }

        private void ShowHide_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                string tag = txtultimosDigitos.Tag.ToString();
                string text = txtultimosDigitos.Text;

                if (ShowHide.Tag.ToString().Equals("no"))
                {
                    txtultimosDigitos.Tag = text;
                    txtultimosDigitos.Text = tag;
                    ShowHide.Tag = "yes";
                    ShowHide.Source = GetImage("no");
                }
                else
                {
                    txtultimosDigitos.Tag = text;
                    txtultimosDigitos.Text = tag;
                    ShowHide.Tag = "no";
                    ShowHide.Source = GetImage("yes");
                }
            }
            catch { }
        }

        private void TextBlock_TouchDown(object sender, TouchEventArgs e)
        {
            NumbersButtons(sender);
        }

        private void BotonAceptar_TouchDown(object sender, TouchEventArgs e)
        {
            string data = ValidateDataMasc();
            if (txtultimosDigitos.Text.Length <= 2 && int.Parse(data) == 0)
            {
                _dataCard.mensaje = "El número de cuotas debe ser mayor que 0.";
                return;
            }
            _dataCard.mensaje = "Esperando datáfono...";
            _dataCard.visible = "Hidden";
            AcceptButton();
        }
    }
}
