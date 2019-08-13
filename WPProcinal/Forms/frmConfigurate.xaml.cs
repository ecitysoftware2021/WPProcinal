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
                        DataPaypad data = JsonConvert.DeserializeObject<DataPaypad>(response.Data.ToString());

                        if (data.State)
                        {
                            if (data.StateAceptance && data.StateDispenser)
                            {
                                if (util == null)
                                {
                                    util = new Utilities(1);
                                }
                                Utilities.control.callbackError = error =>
                                {
                                    //TODO: descomentar
                                    //ShowModalError(error);
                                };
                                Utilities.control.OpenSerialPorts();
                                Utilities.control.callbackToken = isSucces =>
                                {
                                    Dispatcher.BeginInvoke((Action)delegate
                                    {
                                        frmCinema inicio = new frmCinema();
                                        inicio.Show();
                                        Close();
                                    });
                                };
                                Utilities.control.Start();
                            }
                            else
                            {
                                Task.Run(() =>
                                {
                                    if (!string.IsNullOrEmpty(data.Message))
                                    {
                                        SendEmails.SendEmail(data.Message);
                                    }
                                });
                                ShowModalError(Utilities.GetConfiguration("MensajeSinDineroInitial"));
                                GetToken();
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