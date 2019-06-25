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
        int contPanic = 0;

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
                contPanic++;
                if (contPanic > 3)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        frmPanicModal modal = new frmPanicModal();
                        modal.Show();
                    });
                }
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
                                LogService.CreateLogsPeticionRespuestaDispositivos("frmConfigurate", data.Message);
                                ShowModalError(Utilities.GetConfiguration("MensajeSinDineroInitial"));

                                GetToken();
                            }
                        }
                        else
                        {
                            LogService.CreateLogsPeticionRespuestaDispositivos("frmConfigurate", "Estado Perifericos: " + data.State);
                            GetToken();
                            ShowModalError("No se pudo verificar el estado de los periféricos");
                        }
                    }
                    else
                    {
                        LogService.CreateLogsPeticionRespuestaDispositivos("frmConfigurate", response.Message);
                        GetToken();
                        ShowModalError("No se pudo iniciar el Dispositivo");
                    }
                }
                else
                {
                    ShowModalError("No hay conexión disponible.");
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
                frmModal modal = new frmModal(string.Concat("Lo sentimos,", Environment.NewLine, "el cajero no se encuentra disponible.\nError: ", description));
                modal.ShowDialog();
                //if (modal.DialogResult.HasValue)
                //{
                //    frmConfigurate configurate = new frmConfigurate();
                //    configurate.Show();
                //    this.Close();
                //}
            });
        }
    }
}