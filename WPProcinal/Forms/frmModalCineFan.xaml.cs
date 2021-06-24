using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private List<SCOLOGResponse> responseClient;
        #endregion

        #region "Constructor"
        public frmModalCineFan()
        {
            InitializeComponent();
            responseClient = new List<SCOLOGResponse>();
            Utilities.Speack("Bienvenido, si eres un cinefán, escanea tu cédula en el lector!");
            Utilities.dataTransaction.dataUser = new SCOLOGResponse();
        }
        #endregion

        #region "Eventos"
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Utilities.controlScanner.callbackDocument = Document =>
                {
                    if (!string.IsNullOrEmpty(Document.Document))
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            Utilities.dataTransaction.dataDocument = Document;
                            Validate();
                        });

                    }
                };

                Utilities.controlScanner.num = 0;
                Utilities.controlScanner.InitializePortScanner(Utilities.dataPaypad.PaypadConfiguration.scanneR_PORT);
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("Window_Loaded>Window_Loaded", JsonConvert.SerializeObject(ex), 1);
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
            DialogResult = true;
        }
        #endregion

        #region "Métodos"
        private void Validate()
        {
            try
            {
                Task.Run(() => Dispatcher.BeginInvoke((Action)delegate
                {
                    if (ValidateCineFan(Utilities.dataTransaction.dataDocument.Document))
                    {
                        Utilities.controlScanner.callbackDocument = null;
                        Utilities.controlScanner.ClosePortScanner();
                        DialogResult = true;
                    }
                    else
                    {
                        Utilities.controlScanner.num = 0;
                    }
                }));

            }
            catch (Exception ex)
            {
                txtError.Text = "No se reconoció la cédula, intente de nuevo.";
            }
        }

        private bool ValidateCineFan(string cedula)
        {
            try
            {
                frmLoading = new FrmLoading("¡Consultando Cine Fans...!");
                frmLoading.Show();
                responseClient = WCFServices41.GetClientData(new SCOCED
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
                        if (item.Tarjeta != "0")
                        {
                            Utilities.dataTransaction.dataUser = item;
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
                            Correo = Utilities.dataTransaction.dataUser.Login,
                            tercero = "1"
                        });
                        frmLoading.Close();
                        if (saldo != null)
                        {
                            if (saldo.Saldo_Disponible != null)
                            {
                                Utilities.dataTransaction.dataUser.SaldoFavor = saldo.Saldo_Disponible;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        txtError.Text = error;
                        txtActivar.Visibility = Visibility.Visible;
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
                LogService.SaveRequestResponse("Validando el cine fans", JsonConvert.SerializeObject(ex), 1);
                txtError.Text = "No se pudo validar la información, intenta de nuevo.";
                if (frmLoading != null)
                {
                    frmLoading.Close();
                }
                return false;
            }
        }
        #endregion

        private void txtActivar_TouchDown(object sender, TouchEventArgs e)
        {
            txtActivar.Visibility = Visibility.Hidden;
            ActivarMembresia();
        }

        private void ActivarMembresia()
        {
            try
            {
                string error = string.Empty;

                frmLoading = new FrmLoading("¡Consultando secuencia...!");
                frmLoading.Show();

                var responseSEC = WCFServices41.GetSecuence(new SCOSEC
                {
                    Punto = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.AMBIENTE.puntoVenta,
                    teatro = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema,
                    tercero = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.tercero
                });

                frmLoading.Close();
                if (responseSEC != null)
                {
                    frmLoading = new FrmLoading("¡Activando Membresía...!");
                    frmLoading.Show();
                    var response41 = WCFServices41.PostBuy(new SCOINT
                    {
                        Accion = "M",
                        Placa = "0",
                        Apellido = responseClient[0].Apellido,
                        ClienteFrecuente = 0,
                        CorreoCliente = responseClient[0].Login,
                        Cortesia = string.Empty,
                        Direccion = responseClient[0].Direccion,
                        DocIdentidad = long.Parse(responseClient[0].Documento),
                        Factura = int.Parse(responseSEC[0].Secuencia.ToString()),
                        FechaFun = "0",
                        Funcion = 0,
                        InicioFun = 0,
                        Nombre = responseClient[0].Nombre,
                        PagoCredito = 0,
                        PagoEfectivo = 0,
                        PagoInterno = 0,
                        Pelicula = 0,
                        Productos = new List<Producto> { },
                        PuntoVenta = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.AMBIENTE.puntoVenta,
                        Sala = 0,
                        teatro = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema,
                        Telefono = long.Parse(responseClient[0].Telefono),
                        CodMedioPago = 0,
                        tercero = 1,
                        TipoBono = 0,
                        TotalVenta = 0,
                        Ubicaciones = new List<UbicacioneSCOINT> { },
                        Membresia = true,
                        Obs1 = ""
                    });

                    frmLoading.Close();
                    foreach (var item in response41)
                    {
                        if (item.Respuesta != null)
                        {
                            if (item.Respuesta.Contains("exitoso"))
                            {
                                Utilities.controlScanner.callbackDocument = null;
                                Utilities.controlScanner.ClosePortScanner();
                                DialogResult = true;
                                break;
                            }
                            else
                            {
                                error = item.Respuesta;
                            }
                        }
                        else
                        {
                            error = item.Respuesta;
                        }
                    }
                    txtError.Text = error;
                    txtActivar.Visibility = Visibility.Visible;
                }
                else
                {
                    error = "No se pudo obtener la secuencia.";
                    txtActivar.Visibility = Visibility.Visible;
                    txtError.Text = error;
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("Activando membresía", JsonConvert.SerializeObject(ex), 1);
                txtError.Text = "No se pudo activar la membresía.";
                if (frmLoading != null)
                {
                    frmLoading.Close();
                }
            }
        }
    }
}
