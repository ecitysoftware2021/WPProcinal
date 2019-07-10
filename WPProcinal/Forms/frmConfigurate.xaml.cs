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
        public frmConfigurate()
        {
            InitializeComponent();
            api = new ApiLocal();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                GetToken();
            });
        }

        /// <summary>
        /// Método encargado de obtener el token necesario para que el corresponsal pueda operar, seguido de esto se consulta el estado inicial del corresponsal
        /// para saber si se pueden realizar transacciones
        /// </summary>
        private async void GetToken()
        {
            try
            {
                Utilities util = new Utilities(1);
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
                                Utilities.control.callbackError = error =>
                                {
                                    GetToken();
                                };
                                //Utilities.control.callbackToken = isSucces =>
                                //{
                                    Dispatcher.BeginInvoke((Action)delegate
                                            {
                                                if (!Utilities.GetConfiguration("ReImpresion").Equals("si"))
                                                {
                                                    frmCinema inicio = new frmCinema();
                                                    inicio.Show();
                                                    Close();
                                                }
                                                else
                                                {
                                                    frmImpresionForzada impresionForzada = new frmImpresionForzada();
                                                    impresionForzada.Show();
                                                    Close();
                                                }

                                            });
                                //};
                                //Utilities.control.Start();
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
                                LogService.CreateLogsPeticionRespuestaDispositivos("frmConfigurate", data.Message);
                                ShowModalError(Utilities.GetConfiguration("MensajeSinDineroInitial"));
                                GetToken();
                            }
                        }
                        else
                        {
                            LogService.CreateLogsPeticionRespuestaDispositivos("frmConfigurate", "Estado Perifericos: " + data.State);

                            ShowModalError("No se pudo verificar el estado de los periféricos");
                            GetToken();
                        }
                    }
                    else
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("frmConfigurate", response.Message);

                        ShowModalError("No se pudo iniciar el Dispositivo");
                        GetToken();
                    }
                }
                else
                {
                    ShowModalError("No se pudo establecer conexión.");
                    GetToken();
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
                    Utilities.RestartApp();
                }
            });
        }
    }
}