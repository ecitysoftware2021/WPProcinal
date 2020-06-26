using Newtonsoft.Json;
using PrinterValidator;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPProcinal.Classes;
using WPProcinal.Forms.User_Control;
using WPProcinal.Models;
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
        int configurateActive = int.Parse(Utilities.GetConfiguration("ConfigurateActive"));
        Validator validator = new Validator();
        public frmConfigurate()
        {
            var ProcessApp = Process.GetProcessesByName("WPProcinal");
            if (ProcessApp.Length > 1)
            {
                this.Close();
            }

            InitializeComponent();


            if (Directory.Exists("Slider"))
            {
                try
                {
                    foreach (var item in Directory.GetFiles("Slider"))
                    {
                        File.Delete(item);
                    }
                }
                catch { }
            }

            if (!Utilities.GetConfiguration("CashPayState").Equals("1"))
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

            Switcher.Navigator = this;
            try
            {
                api = new ApiLocal();
            }
            catch (Exception ex)
            {
                ShowModalError(ex.Message);
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
                    var response = await api.GetResponse(new Uptake.RequestApi(), "InitPaypad");
                    if (response.CodeError == 200)
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            okServidor.Visibility = Visibility.Visible;
                            badServidor.Visibility = Visibility.Hidden;
                        });

                        Utilities.dataPaypad = JsonConvert.DeserializeObject<DataPaypad>(response.Data.ToString());

                        if (Utilities.dataPaypad.StateUpdate)
                        {
                            ShowModalError("Hay una nueva versión de la aplicación, por favor no manipule ni apague el dispositivo mientras se actualiza.", true);
                        }
                        else if (Utilities.dataPaypad.State)
                        {
                            if (Utilities.GetConfiguration("CashPayState").Equals("1"))
                            {
                                if (Utilities.dataPaypad.StateAceptance && Utilities.dataPaypad.StateDispenser)
                                {
                                    stateMoney = true;
                                    if (util == null)
                                    {
                                        util = new Utilities();
                                    }
                                    if (configurateActive != 1)
                                    {
                                        Utilities.LoadData();
                                        Dispatcher.BeginInvoke((Action)delegate
                                        {
                                            Switcher.Navigate(new UCCinema());
                                        });

                                    }
                                    else
                                    {
                                        ChangeStatusPeripherals();
                                        Task.Run(() =>
                                        {
                                            Utilities.control.OpenSerialPorts();
                                            Utilities.control.Start();
                                            Utilities.control.StartCoinAcceptorDispenser();

                                        });
                                    }

                                }
                                else
                                {
                                    LogService.SaveRequestResponse("Sin dinero", Utilities.dataPaypad.Message, 6);
                                    ShowModalError(Utilities.GetConfiguration("MensajeSinDineroInitial"));
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
                            LogService.SaveRequestResponse("Verificando perifericos", Utilities.dataPaypad.Message, 2);
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
            if (Utilities.GetConfiguration("CashPayState").Equals("1"))
            {
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

            }

            Dispatcher.BeginInvoke((Action)delegate
            {
                peripheralsValidated++;
                okImpresora.Visibility = Visibility.Visible;
                badImpresora.Visibility = Visibility.Hidden;
                CheckPeripheralsAndContinue();
            });


        }

        void SetCallbackNull()
        {
            try
            {
                peripheralsValidated = 0;
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
                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Switcher.Navigate(new UCCinema());
                    });
                });
            }
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
                    GetToken();
                }
                else
                {
                    modal = new frmModal(description, stop);
                    modal.ShowDialog();
                    Utilities.UpdateApp();
                }
            });
        }
    }
}