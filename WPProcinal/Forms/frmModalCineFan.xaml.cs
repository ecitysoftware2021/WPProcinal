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
using WPProcinal.Keyboard;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmModalCineFan.xaml
    /// </summary>
    public partial class frmModalCineFan : Window
    {
        #region "Referencias"
        #endregion

        #region "Constructor"
        public frmModalCineFan()
        {
            InitializeComponent();
            TouchScreenKeyboard.PositionX = 90;
            TouchScreenKeyboard.PositionY = 0;
            TouchScreenKeyNumeric.PositionX = -50;
            Utilities.dataDocument = new Models.DataDocument();
        }
        #endregion

        #region "Eventos"
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Utilities.control.callbackDocument = Document => Dispatcher.BeginInvoke((Action)delegate
                {
                    if (!string.IsNullOrEmpty(Document.Document))
                    {
                        Utilities.dataDocument = Document;
                        Utilities.control.callbackDocument = null;
                        Utilities.control.ClosePortScanner();

                        grdScanner.Visibility = Visibility.Hidden;
                        grdData.Visibility = Visibility.Visible;
                    }
                });

                Utilities.control.num = 0;
                Utilities.control.InitializePortScanner(Utilities.GetConfiguration("PortScanner"));
            }
            catch (Exception ex)
            {
            }
        }

        private void BtnSalir_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

        //Proceso Automatico
        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                EmptyData();

                TextBox textBox = (TextBox)sender;
                string text = textBox.Text;
                int length = text.Length;
                if (length <= 47)
                {

                }
                else
                {
                    textBox.Text = text.Remove(text.Length - 1);
                }
            }
            catch (Exception ex)
            {
            }
        }

        //Proceso Automatico
        private void BtnAceptar_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (ValidarCampos(0))
                {
                    Utilities.dataDocument.Email = txtEmail.Text;
                    Utilities.dataDocument.Phone = txtPhone.Text;
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
            }
        }

        //Proceso Automatico
        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.control.callbackDocument = null;
            Utilities.control.ClosePortScanner();
            DialogResult = false;
        }

        //Proceso Automatico
        private void txtPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                EmptyData();

                TextBox txt = (TextBox)sender;

                if (txt.Text.Length == 1 && txt.Text == "0")
                {
                    txt.Text = string.Empty;
                }

                if (txt.Text.Length > 10)
                {
                    txt.Text = txt.Text.Remove(10, 1);
                    return;
                }
            }
            catch (Exception ex)
            {
            }
        }

        //Proceso Manual
        private void BtnManual_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.control.callbackDocument = null;
            Utilities.control.ClosePortScanner();
            grdScanner.Visibility = Visibility.Hidden;
            grdData.Visibility = Visibility.Hidden;
            grdManual.Visibility = Visibility.Visible;
        }

        //Proceso Manual
        private void BtnAceptar2_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (ValidarCampos(1))
                {
                    Utilities.dataDocument.Email = txtEmail2.Text;
                    Utilities.dataDocument.Phone = txtPhone2.Text;
                    Utilities.dataDocument.Document = txtId.Text;
                    Utilities.dataDocument.FirstName = txtNombre.Text;
                    Utilities.dataDocument.LastName = txtLastName.Text;
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
            }
        }

        //Proceso Manual
        private void BtnCancelar2_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

        //Proceso Automatico
        private void txtChar_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                EmptyData();

                TextBox txt = (TextBox)sender;

                if (txt.Text.Length == 1 && txt.Text == "0")
                {
                    txt.Text = string.Empty;
                }

                if (txt.Text.Length > txt.MaxLength)
                {
                    txt.Text = txt.Text.Remove(txt.MaxLength, 1);
                    return;
                }
            }
            catch (Exception ex)
            {
            }
        }

        //Proceso Automatico
        private void txtId_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                EmptyData();

                TextBox txt = (TextBox)sender;

                if (txt.Text.Length == 1 && txt.Text == "0")
                {
                    txt.Text = string.Empty;
                }

                if (txtId.Text.Length > txt.MaxLength)
                {
                    txtId.Text = txtId.Text.Remove(txt.MaxLength, 1);
                    return;
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region "Métodos"
        private void EmptyData()
        {
            try
            {
                TxtErrorEmail.Text = string.Empty;
                TxtErrorEmail2.Text = string.Empty;
                TxtErrorPhone.Text = string.Empty;
                TxtErrorPhone2.Text = string.Empty;
                TxtErrorLastName.Text = string.Empty;
                TxtErrorName.Text = string.Empty;
                TxtErrorId.Text = string.Empty;
            }
            catch (Exception ex)
            {
            }
        }

        private bool ValidarCampos(int num)
        {
            try
            {
                switch (num)
                {
                    case 0:
                        if (string.IsNullOrEmpty(txtEmail.Text))
                        {
                            TxtErrorEmail.Text = "Debe ingresar un correo electrónico";
                            return false;
                        }
                        else
                        if (!Utilities.IsValidEmailAddress(txtEmail.Text))
                        {
                            TxtErrorEmail.Text = "No se indicó un correo válido";
                            return false;
                        }
                        else
                        if (string.IsNullOrEmpty(txtPhone.Text))
                        {
                            TxtErrorPhone.Text = "Debe ingresar un teléfono";
                            return false;
                        }
                        else
                        if (txtPhone.Text.Length < 7)
                        {
                            TxtErrorPhone.Text = "Debe ingresar un teléfono válido";
                            return false;
                        }
                        break;
                    case 1:
                        if (string.IsNullOrEmpty(txtId.Text))
                        {
                            TxtErrorId.Text = "Debe ingresar una identificación";
                            return false;
                        }
                        if (txtId.Text.Length < 7)
                        {
                            TxtErrorId.Text = "Debe ingresar una identificación válida";
                            return false;
                        }
                        else
                        if (string.IsNullOrEmpty(txtNombre.Text))
                        {
                            TxtErrorName.Text = "Debe ingresar su nombre";
                            return false;
                        }
                        if (txtNombre.Text.Length < 3)
                        {
                            TxtErrorName.Text = "Debe ingresar un nombre válido";
                            return false;
                        }
                        else
                        if (string.IsNullOrEmpty(txtLastName.Text))
                        {
                            TxtErrorLastName.Text = "Debe ingresar su apellido";
                            return false;
                        }
                        if (txtLastName.Text.Length < 7)
                        {
                            TxtErrorLastName.Text = "Debe ingresar un apellido válido";
                            return false;
                        }
                        else
                        if (string.IsNullOrEmpty(txtEmail2.Text))
                        {
                            TxtErrorEmail2.Text = "Debe ingresar un correo electrónico";
                            return false;
                        }
                        else
                        if (!Utilities.IsValidEmailAddress(txtEmail2.Text))
                        {
                            TxtErrorEmail2.Text = "No se indicó un correo válido";
                            return false;
                        }
                        else
                        if (string.IsNullOrEmpty(txtPhone2.Text))
                        {
                            TxtErrorPhone2.Text = "Debe ingresar un teléfono";
                            return false;
                        }
                        else
                        if (txtPhone2.Text.Length < 7)
                        {
                            TxtErrorPhone2.Text = "Debe ingresar un teléfono válido";
                            return false;
                        }
                        break;
                    default:
                        break;
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
