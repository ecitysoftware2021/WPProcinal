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

                        Utilities.control.callbackDocument = null;
                        Utilities.control.ClosePortScanner();
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
            DialogResult = false;
        }
        #endregion

        #region "Métodos"
        private async void Validate()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    BtnSalir.IsEnabled = false;
                });

                Task.Run(() =>
                {
                    if (ValidateCineFan(Utilities.dataDocument.Document))
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            DialogResult = true;
                        });
                    }
                });

            }
            catch (Exception ex)
            {
            }
        }

        private bool ValidateCineFan(string cedula)
        {
            try
            {
                {
                    var responseClient = WCFServices41.GetClientData(new SCOCED
                    {
                        Documento = long.Parse(cedula),
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
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                txtError.Text = responseClient.Estado;
                            });
                            return false;
                        }
                    }
                    else
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            txtError.Text = "No se pudo validar la informacion.";
                        });
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

    }
}
