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
        int PeripheralsToCheck = int.Parse(Utilities.GetConfiguration("PeripheralsToCheck"));
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
                                stateMoney = true;
                                if (util == null)
                                {
                                    util = new Utilities(1);
                                }


                                ChangeStatusPeripherals();

                                Task.Run(() =>
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
                ShowModalError(ex.Message);
            }
        }

        private void ChangeStatusPeripherals()
        {

            if (stateMoney)
            {
                peripheralsValidated++;
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
                if (Status.STATUS == "OK" || Status.STATUS == "ALERT")
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
                        AdminPaypad.SaveErrorControl(Status.ERROR_MESSAGE,
                            "Respuesta de la impresora",
                            EError.Device,
                            ELevelError.Medium);
                    }
                    catch { }
                    ShowModalError(Status.ERROR_MESSAGE);
                }
            };
        }

        void SetCallbackNull()
        {
            try
            {
                peripheralsValidated = 0;
                ControlPeripheralsNotArduino.callbackStatusPrinter = null;
                Utilities.control.callbackToken = null;
                Utilities.control.callbackStatusCoinAceptanceDispenser = null;
                Utilities.control.callbackStatusBillAceptance = null;
                Utilities.control.callbackError = null;
            }
            catch { }
        }

        private void CheckPeripheralsAndContinue()
        {
            if (peripheralsValidated >= PeripheralsToCheck)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    frmCinema inicio = new frmCinema();
                    inicio.Show();
                    Close();
                });
            }
        }

        private void ShowModalError(string description)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                SetCallbackNull();
                frmModal modal = new frmModal(string.Concat("Lo sentimos,", Environment.NewLine, "el dispositivo no se encuentra disponible.\nMensaje: ", description));
                modal.ShowDialog();
                GetToken();
            });
        }
    }
}