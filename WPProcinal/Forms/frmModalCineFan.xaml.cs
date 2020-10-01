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

        private void BtnRegister_TouchDown(object sender, TouchEventArgs e)
        {
            frmRegister frmRegister = new frmRegister();
            frmRegister.ShowDialog();
            if (frmRegister.DialogResult.HasValue && frmRegister.DialogResult.Value)
            {
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
        }

        private void Image_TouchDown(object sender, TouchEventArgs e)
        {
            DialogResult = false;
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

                frmLoading = new FrmLoading("¡Consultando Cine Fans...!");
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
                            frmLoading = new FrmLoading("¡Consultando Puntos Cine Fans...!");
                            frmLoading.Show();
                            DataService41.dataUser.Puntos = WCFServices41.ConsultPoints(new SCOMOV
                            {
                                Correo = DataService41.dataUser.Login,
                                Clave = DataService41.dataUser.Clave,
                                tercero = 1
                            });
                            frmLoading.Close();
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
                        frmLoading = new FrmLoading("¡Consultando Saldo Cine Fans...!");
                        frmLoading.Show();
                        var saldo = WCFServices41.GetPersonBalance(new SCOSDO
                        {
                            Correo = DataService41.dataUser.Login,
                            tercero = "1"
                        });
                        frmLoading.Close();
                        if (saldo != null)
                        {
                            if (saldo.Saldo_Disponible != null)
                            {
                                DataService41.dataUser.SaldoFavor = saldo.Saldo_Disponible;
                            }
                        }
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
                LogService.SaveRequestResponse("Validando el cine fans", ex.Message, 1);
                txtError.Text = "No se pudo validar la información, intenta de nuevo.";
                frmLoading.Close();
                return false;
            }
        }
        #endregion

    }
}
