using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPProcinal.ADO;
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
                    ShowModalError("No hay conexión disponible.");
                }
            }
            catch (Exception ex)
            {
                ShowModalError(ex.Message, ex.StackTrace);
            }
        }

        public void NotifyPending()
        {
            try
            {
                using (var con = new DBProcinalEntities())
                {
                    var notifies = con.NotifyPay.ToList();

                    foreach (var item in notifies)
                    {
                        Transaction Transaction = new Transaction
                        {
                            STATE_TRANSACTION_ID = item.STATE_TRANSACTION_ID.Value,
                            DATE_END = item.DATE_END.Value,
                            INCOME_AMOUNT = item.INCOME_AMOUNT.Value,
                            RETURN_AMOUNT = item.RETURN_AMOUNT.Value,
                            TRANSACTION_ID = item.TRANSACTION_ID.Value
                        };

                        var response = api.GetResponse(new Uptake.RequestApi()
                        {
                            Data = Transaction
                        }, "UpdateTransaction");

                        if (response != null)
                        {
                            if (response.Result.CodeError == 200)
                            {
                                con.NotifyPay.Remove(item);
                                con.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void ShowModalError(string description, string message = "")
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                frmModal modal = new frmModal(string.Concat("Lo sentimos,", Environment.NewLine, "el cajero no se encuentra disponible.\nError: ", description));
                modal.ShowDialog();
                if (modal.DialogResult.HasValue)
                {
                    frmConfigurate configurate = new frmConfigurate();
                    configurate.Show();
                    this.Close();
                }
            });
        }
    }
}