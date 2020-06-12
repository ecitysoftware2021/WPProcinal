using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPProcinal.Classes;
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
            Utilities.dataDocument = new Models.DataDocument();
            DataService41.dataUser = new SCOLOGResponse();

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
            DialogResult = false;
            //DialogResult = ValidateCineFan("1028040734");
            // DialogResult = ValidateCineFan("43261286");
            //DialogResult = ValidateCineFan("0071949041");
            //DialogResult = ValidateCineFan("27955585");
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
                cedula = long.Parse(cedula).ToString();

                frmLoading = new FrmLoading("¡Consultando información...!");
                frmLoading.Show();
                var responseClient = WCFServices41.GetClientData(new SCOCED
                {
                    Documento = cedula,
                    tercero = "1"
                });
                frmLoading.Close();
                bool isCineFan = false;
                string error = string.Empty;
                if (responseClient != null)
                {
                    foreach (var item in responseClient)
                    {
                        if (item.Tarjeta != null)
                        {
                            DataService41.dataUser = item;
                            DataService41.dataUser.Puntos = WCFServices41.ConsultPoints(new SCOMOV
                            {
                                Correo = DataService41.dataUser.Login,
                                Clave = DataService41.dataUser.Clave,
                                tercero = 1
                            });
                            isCineFan = true;
                            break;
                        }
                        else
                        {
                            error = item.Estado != null ? item.Estado : "Usuario no registrado en el sistema.";
                        }
                    }
                    if (isCineFan)
                    {
                        return true;
                    }
                    else
                    {
                        txtError.Text = error;
                        return false;
                    }
                }
                else
                {
                    txtError.Text = "No se pudo validar la información, intenta de nuevo.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("Validando el cinefan", ex.Message, 1);
                txtError.Text = "No se pudo validar la información, intenta de nuevo.";
                frmLoading.Close();
                return false;
            }
        }
        #endregion

    }
}
