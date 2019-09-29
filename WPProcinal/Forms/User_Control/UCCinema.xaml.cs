using Grabador.Transaccion;
using PrinterValidator;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCCinema.xaml
    /// </summary>
    public partial class UCCinema : UserControl
    {
        ApiLocal api;
        Utilities printService;
        static CLSGrabador grabador = new CLSGrabador();
        public UCCinema()
        {
            InitializeComponent();
            printService = new Utilities();
            api = new ApiLocal();
            Utilities.CinemaId = Utilities.GetConfiguration("CodCinema");
            try
            {
                grabador.FinalizarGrabacion();
            }
            catch { }
            if (Utilities.LossConnection)
            {
                Utilities.RestartApp();
            }
            Task.Run(() =>
            {
                SendPayments();
            });

            try
            {
                Process.Start(Path.Combine(Directory.GetCurrentDirectory(), "Renotificar", "RenotifyConsole.exe"));
            }
            catch { }

            LoadData();
        }

        private void SendPayments()
        {
            try
            {
                var notpayments = DBProcinalController.GetDipmapNotPay();
                if (notpayments != null)
                {
                    foreach (var notpayment in notpayments)
                    {
                        var dipmap = DBProcinalController.GetDipMap2(notpayment.DipMapId.Value);
                        if (dipmap != null)
                        {
                            var seats = DBProcinalController.GetSeats(dipmap.DipMapId);
                            var total = seats.Select(s => s.Price).Sum();
                            var response = WCFServices.PostComprar(dipmap, double.Parse(total.Value.ToString()), "E", seats.Count());
                            if (response.IsSuccess)
                            {
                                var res = DBProcinalController.UpdateDipmapNotPay(dipmap.DipMapId);
                                var res2 = DBProcinalController.UpdateDipMap(dipmap.DipMapId);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "SendPayments en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

        private void LoadData()
        {
            try
            {
                Utilities.LstMovies.Clear();
                this.Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    string dataXml = string.Empty;
                    if (!Utilities.IfExistFileXML())
                    {
                        var response = WCFServices.DownloadData();
                        if (!response.IsSuccess)
                        {
                            Utilities.ShowModal(response.Message);
                            return;
                        }

                        Utilities.SaveFileXML(response.Result.ToString());
                        dataXml = response.Result.ToString();
                    }
                    else
                    {
                        dataXml = Utilities.GetFileXML();
                    }

                    Peliculas data = WCFServices.DeserealizeXML<Peliculas>(dataXml);
                    Utilities.Peliculas = data;
                }));
            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "LoadData en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

        private void gridPrincipal_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                gridPrincipal.IsEnabled = false;
                ControlPeripheralsNotArduino.callbackStatusPrinter = Status =>
                {
                    ControlPeripheralsNotArduino.callbackStatusPrinter = null;
                    if (Status.STATUS == "OK")
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            Switcher.Navigate(new UCMovies());
                        });
                    }
                    else if (Status.STATUS == "ALERT")
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            frmModal modal = new frmModal(string.Concat("Alerta,", Environment.NewLine, "Impresora: ", Status.ERROR_MESSAGE));
                            modal.ShowDialog();
                            Switcher.Navigate(new UCMovies());
                        });
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
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            gridPrincipal.IsEnabled = true;
                            frmModal modal = new frmModal(Status.ERROR_MESSAGE);
                            modal.ShowDialog();
                        });
                    }
                };

                Validator validator = new Validator();


                Utilities.PeripheralsNotArduino.ProcessResponsePrinter(validator.ValidatePrinter());


            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "BtnConsult en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }
    }
}
