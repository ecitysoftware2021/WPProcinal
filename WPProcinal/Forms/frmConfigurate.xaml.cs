using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPProcinal.Classes;
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

                                ChangeStatusPeripherals();
                                Task.Run(() =>
                                {
                                    if (string.IsNullOrEmpty(Utilities.dataPaypad.PaypadConfiguration.unifieD_PORT))
                                    {
                                        Utilities.control.OpenSerialPorts(
                                            Utilities.dataPaypad.PaypadConfiguration.bilL_PORT,
                                            Utilities.dataPaypad.PaypadConfiguration.coiN_PORT);
                                        Utilities.control.Start();
                                        Utilities.control.StartCoinAcceptorDispenser();
                                    }
                                    else
                                    {
                                        Utilities.controlUnified.OpenSerialPorts(Utilities.dataPaypad.PaypadConfiguration.unifieD_PORT);
                                        Utilities.controlUnified.Start();
                                    }
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
                    if (string.IsNullOrEmpty(Utilities.dataPaypad.PaypadConfiguration.unifieD_PORT))
                    {
                        Utilities.control.callbackError = (error, description, EError, ELEvelError) =>
                        {
                            Utilities.control.callbackError = null;
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                Utilities.control.callbackError = null;
                            });
                            AdminPaypad.SaveErrorControl(error, description, (EError)EError, (ELevelError)ELEvelError);
                            ShowModalError(error);
                        };

                        Utilities.control.CallBackSaveRequestResponse = (Title, Message, State) =>
                        {
                            LogService.SaveRequestResponse(Title, Message, State);
                        };


                        Utilities.control.callbackToken = isSucces =>
                        {
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                Utilities.control.callbackToken = null;
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
                        Utilities.controlUnified.callbackError = (error, description, EError, ELEvelError) =>
                        {
                            Utilities.controlUnified.callbackError = null;
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                Utilities.controlUnified.callbackError = null;
                            });
                            AdminPaypad.SaveErrorControl(error, description, (EError)EError, (ELevelError)ELEvelError);
                            ShowModalError(error);
                        };

                        Utilities.controlUnified.CallBackSaveRequestResponse = (Title, Message, State) =>
                        {
                            LogService.SaveRequestResponse(Title, Message, State);
                        };


                        Utilities.controlUnified.callbackToken = isSucces =>
                        {
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                Utilities.controlUnified.callbackToken = null;
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

        void SetCallbackNull()
        {
            try
            {
                if (Utilities.control != null)
                {
                    Utilities.control.callbackToken = null;
                    Utilities.control.callbackError = null;
                }
                else if (Utilities.controlUnified != null)
                {
                    Utilities.controlUnified.callbackToken = null;
                    Utilities.controlUnified.callbackError = null;
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
                Thread.Sleep(1000);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Switcher.Navigate(new UCCinema());
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
                    GetToken();
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