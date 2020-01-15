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
using WPProcinal.Service;

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
            Utilities.dataDocument = new Models.DataDocument();
            Utilities.dataUser = new SCOLOGResponse();
        }
        #endregion

        #region "Eventos"
        private void BtnSalir_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
        }

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

        private void BtnContinuar_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    Utilities.dataDocument.Email = txtEmail.Text;

                    if (ValidateCineFan(txtEmail.Text))
                    {
                        DialogResult = true;
                    }
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
            }
            catch (Exception ex)
            {
            }
        }

        private bool ValidarCampos()
        {
            try
            {
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
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool ValidateCineFan(string mail)
        {
            var response = WCFServices41.GetUserKey(new SCOCSN
            {
                Correo = mail,
                teatro = Utilities.GetConfiguration("CodCinema"),
                tercero = "1"
            });
            if (response.Split(' ').Count() > 1)
            {
                TxtErrorEmail.Text = response;
                return false;
            }
            else
            {
                var responseClient = WCFServices41.GetClientData(new SCOLOG
                {
                    Clave = response,
                    Correo = mail,
                    teatro = Utilities.GetConfiguration("CodCinema"),
                    tercero = "1"
                });
                if (responseClient != null)
                {
                    if (responseClient.Tarjeta != null)
                    {
                        Utilities.dataUser = responseClient;
                        return true;
                    }
                    else
                    {
                        TxtErrorEmail.Text = responseClient.Estado;
                        return false;
                    }
                }
                else
                {
                    TxtErrorEmail.Text = "No se pudo validar la informacion.";

                    return false;
                }
            }
        }
        #endregion

    }
}
