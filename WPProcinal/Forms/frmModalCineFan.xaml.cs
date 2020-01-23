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
        FrmLoading frmLoading;
        #endregion

        #region "Constructor"
        public frmModalCineFan()
        {
            InitializeComponent();
            TouchScreenKeyboard.PositionX = 90;
            TouchScreenKeyboard.PositionY = 0;
            Utilities.dataDocument = new Models.DataDocument();
            Utilities.dataUser = new SCOLOGResponse();

            Utilities.Speack("Bienvenido, si eres un cinefán, escanea tu cédula en el lector!");
        }
        #endregion

        #region "Eventos"
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Utilities.control.callbackDocument = Document =>
                {
                    if (!string.IsNullOrEmpty(Document.Document))
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            Utilities.dataDocument = Document;
                            Validate();
                        });

                    }
                };

                Utilities.control.num = 0;
                Utilities.control.InitializePortScanner(Utilities.GetConfiguration("PortScanner"));
            }
            catch (Exception ex)
            {
            }
        }

        private void BtnSalir_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.control.callbackDocument = null;
            Utilities.control.ClosePortScanner();
            //DialogResult = false;
            DialogResult = ValidateCineFan("1028040734");
        }
        #endregion

        #region "Métodos"
        private async void Validate()
        {
            try
            {
                Task.Run(() => Dispatcher.BeginInvoke((Action)delegate
                {
                    if (ValidateCineFan(Utilities.dataDocument.Document))
                    {
                        Utilities.control.callbackDocument = null;
                        Utilities.control.ClosePortScanner();
                        DialogResult = true;
                    }
                    else
                    {
                        Utilities.control.num = 0;
                    }
                }));

            }
            catch (Exception ex)
            {
            }
        }

        private bool ValidateCineFan(string cedula)
        {
            try
            {

                frmLoading = new FrmLoading("¡Consultando información...!");
                frmLoading.Show();
                var responseClient = WCFServices41.GetClientData(new SCOCED
                {
                    Documento = cedula,
                    tercero = "1"
                });
                frmLoading.Close();
                if (responseClient != null)
                {
                    if (responseClient.Tarjeta != null)
                    {
                        Utilities.dataUser = responseClient;
                        Utilities.dataUser.Puntos = WCFServices41.ConsultPoints();
                        return true;
                    }
                    else
                    {
                        txtError.Text = responseClient.Estado != null ? responseClient.Estado : "Usuario no registrado en el sistema.";
                        return false;
                    }
                }
                else
                {
                    txtError.Text = "No se pudo validar la información.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    AdminPaypad.SaveErrorControl(ex.Message,
                    "ValidateCineFan en frmModalCineFan",
                    EError.Aplication,
                    ELevelError.Mild);
                }
                catch { }
                txtError.Text = "No se pudo validar la información.";
                frmLoading.Close();
                return false;
            }
        }
        #endregion

    }
}
