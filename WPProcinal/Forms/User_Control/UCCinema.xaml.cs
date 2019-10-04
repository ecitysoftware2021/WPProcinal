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
        static CLSGrabador grabador = new CLSGrabador();
        public UCCinema()
        {
            InitializeComponent();
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

            try
            {
                Process.Start(Path.Combine(Directory.GetCurrentDirectory(), "Renotificar", "RenotifyConsole.exe"));
            }
            catch { }

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                Utilities.LstMovies.Clear();
                this.Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    string dataXml = string.Empty;

                    var response = WCFServices41.DownloadData();
                    if (!response.IsSuccess)
                    {
                        Utilities.ShowModal(response.Message);
                        return;
                    }

                    Utilities.SaveFileXML(response.Result.ToString());
                    dataXml = response.Result.ToString();

                    dataXml = Utilities.GetFileXML();
                    Peliculas data = WCFServices41.DeserealizeXML<Peliculas>(dataXml);
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

        private void Efectivo_TouchDown(object sender, TouchEventArgs e)
        {
            frmStoryBoard storyBoard = new frmStoryBoard("PagoEfectivo.mp4");
            storyBoard.ShowDialog();
        }

        private void Debito_TouchDown(object sender, TouchEventArgs e)
        {

            frmStoryBoard storyBoard = new frmStoryBoard("PagoDebito.mp4");
            storyBoard.ShowDialog();
        }

        private void Credito_TouchDown(object sender, TouchEventArgs e)
        {

            frmStoryBoard storyBoard = new frmStoryBoard("PagoCredito.mp4");
            storyBoard.ShowDialog();
        }
    }
}
