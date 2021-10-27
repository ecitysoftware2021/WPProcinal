using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPProcinal.Classes;
using WPProcinal.Classes.Peripherals;
using WPProcinal.Forms.User_Control;
using WPProcinal.Service;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for ConfigurateWindow.xaml
    /// </summary>
    public partial class frmConfigurate : Window
    {
        ApiLocal api;
        bool state;
        bool status = true;
        Utilities util;

        public frmConfigurate()
        {
            InitializeComponent();

            try
            {
                Switcher.Navigator = this;
                api = new ApiLocal();
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmConfigurate>frmConfigurate", JsonConvert.SerializeObject(ex), 1);
                ShowModalError(ex.Message);
            }
        }

        private void SetVisibility()
        {
            try
            {
                if (!Utilities.dataPaypad.PaypadConfiguration.enablE_VALIDATE_PERIPHERALS)
                {
                    okAceptadorBilletes.Visibility = Visibility.Hidden;
                    badAceptadorBilletes.Visibility = Visibility.Hidden;
                    lblBillAceptance.Visibility = Visibility.Hidden;

                    okMonederos.Visibility = Visibility.Hidden;
                    badMonederos.Visibility = Visibility.Hidden;
                    lblCoins.Visibility = Visibility.Hidden;


                    okDispensadorBilletes.Visibility = Visibility.Hidden;
                    badDispensadorBilletes.Visibility = Visibility.Hidden;
                    lblBillDispenser.Visibility = Visibility.Hidden;

                    Grid.SetRow(lblPrinter, 2);
                    Grid.SetRow(okImpresora, 2);
                    Grid.SetRow(badImpresora, 2);

                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmConfigurate>SetVisibility", JsonConvert.SerializeObject(ex), 1);
            }
        }

        /// <summary>
        /// Método que asigna esta ventana (Windows) como la principal para los usercontrol
        /// </summary>
        /// <param name="pagina"></param>
        public void Navegar(UserControl newPage)
        {
            this.Content = null;
            this.Content = newPage;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (api != null)
            {
                GetToken();
            }
        }

        /// <summary>
        /// Método encargado de obtener el token necesario para que el corresponsal pueda operar, seguido de esto se consulta el estado inicial del corresponsal
        /// para saber si se pueden realizar transacciones
        /// </summary>
        private async void GetToken()
        {
            try
            {
                state = await api.SecurityToken();
                if (state)
                {
                    await AdminPaypad.UpdatePeripherals();

                    if (Utilities.dataPaypad.State)
                    {
                        ConfigurePublicity();
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            okServidor.Visibility = Visibility.Visible;
                            badServidor.Visibility = Visibility.Hidden;
                        });

                        SetVisibility();
                        if (Utilities.dataPaypad.StateUpdate)
                        {
                            ShowModalError("Hay una nueva versión de la aplicación, por favor no manipule ni apague el dispositivo mientras se actualiza.", true);
                            return;
                        }

                        if (Utilities.dataPaypad.PaypadConfiguration.enablE_VALIDATE_PERIPHERALS)
                        {
                            if (Utilities.dataPaypad.StateAceptance && Utilities.dataPaypad.StateDispenser)
                            {
                                if (util == null)
                                {
                                    util = new Utilities();
                                }

                                Task.Run(() =>
                                {
                                    Utilities.control = new ControlPeripherals(Utilities.dataPaypad.PaypadConfiguration.unifieD_PORT,
                                        Utilities.dataPaypad.PaypadConfiguration.dispenseR_CONFIGURATION);

                                    ChangeStatusPeripherals();

                                    Utilities.control.Start();

                                });
                            }
                            else
                            {
                                LogService.SaveRequestResponse("Sin dinero", Utilities.dataPaypad.Message, 6);
                                ShowModalError(Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.MensajeSinDineroInitial);
                            }
                        }
                        else
                        {
                            if (util == null)
                            {
                                util = new Utilities();
                            }

                            ChangeStatusPeripherals();
                        }

                    }
                    else
                    {
                        ShowModalError("No se pudo iniciar el Dispositivo");
                    }
                }
                else
                {
                    ShowModalError("No se pudo establecer conexión.");
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmConfigurate>GetToken", JsonConvert.SerializeObject(ex), 1);
                ShowModalError(ex.Message);
            }
        }

        private void ConfigurePublicity()
        {
            try
            {
                var slides = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.PublicityRoute;
                if (Directory.Exists(slides))
                {
                    foreach (var item in Directory.GetFiles(slides))
                    {
                        File.Delete(item);
                    }
                }
                else
                {
                    Directory.CreateDirectory(slides);
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmConfigurate>ConfigurePublicity", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void ChangeStatusPeripherals()
        {
            try
            {
                if (Utilities.dataPaypad.PaypadConfiguration.enablE_VALIDATE_PERIPHERALS)
                {
                    Utilities.control.callbackError = error =>
                    {
                        Utilities.control.callbackError = null;
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            status = false;
                            Utilities.control.callbackError = null;
                        });

                        AdminPaypad.SaveErrorControl(error, "", Classes.EError.Device, ELevelError.Medium);
                        
                        ShowModalError(SetMessageError(error));
                        //ShowModalError(error);
                    };

                    Utilities.control.CallBackSaveRequestResponse = (Title, Message, State) =>
                    {
                        LogService.SaveRequestResponse(Title, Message, State);
                    };

                    Utilities.control.callbackToken = isSucces =>
                    {
                        //Utilities.control.callbackError = null;

                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            status = true;
                            Utilities.control.callbackToken = null;
                            //Utilities.control.callbackError = null;

                            okAceptadorBilletes.Visibility = Visibility.Visible;
                            badAceptadorBilletes.Visibility = Visibility.Hidden;

                            okMonederos.Visibility = Visibility.Visible;
                            badMonederos.Visibility = Visibility.Hidden;

                            okDispensadorBilletes.Visibility = Visibility.Visible;
                            badDispensadorBilletes.Visibility = Visibility.Hidden;

                            okImpresora.Visibility = Visibility.Visible;
                            badImpresora.Visibility = Visibility.Hidden;
                            CheckPeripheralsAndContinue();
                        });
                    };
                }
                else
                {
                    CheckPeripheralsAndContinue();
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmConfigurate>ChangeStatusPeripherals", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private string SetMessageError(string mesagge) 
        {
            string msgError = string.Empty;

            try
            {
                switch (mesagge)
                {
                    case var s when mesagge.Contains("Empty or jam in 1 cassette"):
                        msgError = "casete 1 vacio o atascado";
                        break;
                    case var s when mesagge.Contains("Empty or jam in 2 cassette"):
                        msgError = "casete 2 vacio o atascado";
                        break;
                    case var s when mesagge.Contains("Empty or jam in 3 cassette"):
                        msgError = "casete 3 vacio o atascado";
                        break;
                    case var s when mesagge.Contains("Empty or jam in 4 cassette"):
                        msgError = "casete 4 vacio o atascado";
                        break;
                    case var s when mesagge.Contains("Empty or jam in 5 cassette"):
                        msgError = "casete 5 vacio o atascado";
                        break;
                    case var s when mesagge.Contains("sensor"):
                        msgError = "Error de atasco, revisar billete atascado";
                        break;
                    case var s when mesagge.Contains("Und"):
                        msgError = "Error Indefinido, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("notes reject error"):
                        msgError = "Error de rechazo continuo de varios billetes, revisar el estado de los billetes";
                        break;
                    case var s when mesagge.Contains("Dispense retry-over error"):
                        msgError = "Error al reintentar dispensar, comuniquese con soporte tecnico";
                        break;
                    case var s when mesagge.Contains("Reject box switch error"):
                        msgError = "Error del interruptor de la caja de rechazo, no esta puesta la caja de rechazo";
                        break;
                    case var s when mesagge.Contains("CIS open switch error"):
                        msgError = "Error de interruptor abierto CIS, verifique la parte movil trasera del billetero";
                        break;
                    case var s when mesagge.Contains("Solenoid EXIT direction error"):
                        msgError = "Error de dirección de SALIDA del solenoide, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("Solenoid Reject direction error"):
                        msgError = "Error de dirección de rechazo de solenoide, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("Jam at main feed motor"):
                        msgError = "Atasco en el motor de alimentación principal, revisar billete atascado.";
                        break;
                    case var s when mesagge.Contains("Validator setting error"):
                        msgError = "Error de configuración del validador, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("No reponse from validator"):
                        msgError = "Sin respuesta del validador, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("Validator response order error"):
                        msgError = "Error de orden de respuesta del validador, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("Validator response error (BCC inconsistency)"):
                        msgError = "Error de respuesta del validador (inconsistencia de BCC), comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("Validator CIS calibration error"):
                        msgError = "Error de calibración CIS del validador, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("Validator F/W assign error"):
                        msgError = "Error de asignación de F / W del validador, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("Validator result reception timeout error"):
                        msgError = "Error de tiempo de espera de recepción del resultado del validador, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("Validator image reception order error"):
                        msgError = "Error de orden de recepción de la imagen del validador, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("Dispense speed input error"):
                        msgError = "Error de entrada de velocidad de dispensación, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("Number of cassettes input error"):
                        msgError = "Error de entrada de número de casetes, verifique que este bien ingresado el caset";
                        break;
                    case var s when mesagge.Contains("F/W download"):
                        msgError = "Error de descarga de F / W, comuniquese con soporte tecnico.";
                        break;
                    case var s when mesagge.Contains("Dispense direcction timeout error"):
                        msgError = "Error de tiempo de espera de dirección de dispensación";
                        break;
                    case var s when mesagge.Contains("Sub-register command and response"):
                        msgError = "Comando y respuesta de subregistro, comuniquese con soporte tecnico.";
                        break;
                    default:
                        msgError = mesagge;
                        break;
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmConfigurate>SetMessageError", JsonConvert.SerializeObject(ex), 1);
            }
            return msgError;
        }

        void SetCallbackNull()
        {
            try
            {
                if (Utilities.control != null)
                {
                    Utilities.control.callbackToken = null;
                    Utilities.control.callbackError = null;
                    Utilities.control.CloseCallbackAP();
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("frmConfigurate>SetCallbackNull", JsonConvert.SerializeObject(ex), 1);
            }
        }

        private void CheckPeripheralsAndContinue()
        {
            Task.Run(() =>
            {
                Thread.Sleep(3000);

                Dispatcher.BeginInvoke((Action)delegate
                {
                    if (status)
                    {
                        SetCallbackNull();
                        Switcher.Navigate(new UCCinema());
                    }
                });
            });
        }

        private void ShowModalError(string description, bool stop = false)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                SetCallbackNull();
                frmModal modal;

                if (!stop)
                {
                    modal = new frmModal(string.Concat("Lo sentimos,", Environment.NewLine, "el dispositivo no se encuentra disponible.\nMensaje: ", description), stop);
                    modal.ShowDialog();
                    //GetToken();
                    Utilities.RestartApp();
                }
                else
                {
                    modal = new frmModal(description, stop);
                    modal.Show();
                    Utilities.UpdateApp();
                }
            });
        }
    }
}