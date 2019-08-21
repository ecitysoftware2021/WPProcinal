using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Windows;
using WPProcinal.Classes;
using WPProcinal.Models.ApiLocal;
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
        Utilities util;
        int peripheralsValidated = 0;
        bool stateMoney = false;
        public frmConfigurate()
        {
            InitializeComponent();
            try
            {
                api = new ApiLocal();
            }
            catch (Exception ex)
            {
                ShowModalError(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (api != null)
            {
                Task.Run(() =>
                    {
                        GetToken();
                    });
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
                    var response = await api.GetResponse(new Uptake.RequestApi(), "InitPaypad");
                    if (response.CodeError == 200)
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            okServidor.Visibility = Visibility.Visible;
                            badServidor.Visibility = Visibility.Hidden;
                        });
                        DataPaypad data = JsonConvert.DeserializeObject<DataPaypad>(response.Data.ToString());

                        if (data.State)
                        {
                            if (data.StateAceptance && data.StateDispenser)
                            {
                                Dispatcher.BeginInvoke((Action)delegate
                                {
                                    stateMoney = true;
                                });
                                if (util == null)
                                {
                                    util = new Utilities(1);
                                    ChangeStatusPeripherals();
                                }
                                Dispatcher.BeginInvoke((Action)delegate
                                {
                                    Utilities.control.OpenSerialPorts();
                                    Utilities.control.Start();
                                    Utilities.control.StartCoinAcceptorDispenser();
                                    Utilities.PeripheralsNotArduino.InitPortPrinter();
                                });
                            }
                            else
                            {
                                ShowModalError(Utilities.GetConfiguration("MensajeSinDineroInitial"));
                            }
                        }
                        else
                        {
                            ShowModalError("No se pudo verificar el estado de los periféricos");
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
                ShowModalError(ex.Message, ex.StackTrace);
            }
        }

        private void ChangeStatusPeripherals()
        {

            if (stateMoney)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    peripheralsValidated++;
                });
            }

            Utilities.control.callbackError = error =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Utilities.control.callbackError = null;
                });
                ShowModalError(error);
            };

            Utilities.control.callbackStatusBillAceptance = State =>
            {

                Dispatcher.BeginInvoke((Action)delegate
                {
                    Utilities.control.callbackStatusBillAceptance = null;
                    peripheralsValidated++;
                    okAceptadorBilletes.Visibility = Visibility.Visible;
                    badAceptadorBilletes.Visibility = Visibility.Hidden;
                    CheckPeripheralsAndContinue();
                });
            };

            Utilities.control.callbackStatusCoinAceptanceDispenser = State =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Utilities.control.callbackStatusCoinAceptanceDispenser = null;
                    peripheralsValidated++;
                    okMonederos.Visibility = Visibility.Visible;
                    badMonederos.Visibility = Visibility.Hidden;
                    CheckPeripheralsAndContinue();
                });
            };

            Utilities.control.callbackToken = isSucces =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Utilities.control.callbackToken = null;
                    peripheralsValidated++;
                    okDispensadorBilletes.Visibility = Visibility.Visible;
                    badDispensadorBilletes.Visibility = Visibility.Hidden;
                    CheckPeripheralsAndContinue();
                });
            };

            ControlPeripheralsNotArduino.callbackStatusPrinter = Status =>
            {
                if (Status.STATUS == "OK")
                {
                    ControlPeripheralsNotArduino.callbackStatusPrinter = null;
                    peripheralsValidated++;
                    okImpresora.Visibility = Visibility.Visible;
                    badImpresora.Visibility = Visibility.Hidden;
                    CheckPeripheralsAndContinue();
                }
                else
                {
                    try
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos(DateTime.Now + " :: Respuesta de la Impresora: ", Status.ERROR_MESSAGE);
                    }
                    catch { }
                }
            };
        }

        private void CheckPeripheralsAndContinue()
        {
            if (peripheralsValidated >= 5)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    frmCinema inicio = new frmCinema();
                    inicio.Show();
                    Close();
                });
            }
        }

        private void ShowModalError(string description, string message = "")
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                frmModal modal = new frmModal(string.Concat("Lo sentimos,", Environment.NewLine, "el dispositivo no se encuentra disponible.\nMensaje: ", description));
                modal.ShowDialog();
                if (modal.DialogResult.HasValue)
                {
                    GetToken();
                }
            });
        }
        private void ShowModalError(string description)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                frmModal modal = new frmModal(string.Concat("Lo sentimos,", Environment.NewLine, "el dispositivo no se encuentra disponible.\nMensaje: ", description));
                modal.ShowDialog();
                if (modal.DialogResult.HasValue)
                {
                    Utilities.RestartApp();
                }
            });
        }
    }
}