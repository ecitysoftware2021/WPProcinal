using Grabador.Transaccion;
using PrinterValidator;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
        private FrmLoading frmLoading;
        private ImageSleader _imageSleader;
        System.Timers.Timer timerStatePay = new System.Timers.Timer();

        public UCCinema()
        {
            InitializeComponent();
            Utilities.SelectedChairs = new System.Collections.Generic.List<ChairsInformation>();
            DataService41._Combos = new System.Collections.Generic.List<Combos>();
            timerStatePay.Interval = 10000;
            timerStatePay.Elapsed += new System.Timers.ElapsedEventHandler(TimerStatePay_Tick);

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
                var pat = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Renotificar", "RenotifyConsole.exe");
                Process.Start(pat);
            }
            catch { }
            frmLoading = new FrmLoading("¡Descargando información...!");


            Task.Run(() => Dispatcher.BeginInvoke((Action)delegate
            {
                frmLoading.Show();
                Utilities.LoadData();
                frmLoading.Close();
                timerStatePay.Start();
                ConfiguratePublish();
            }));

        }

        private void TimerStatePay_Tick(object sender, ElapsedEventArgs e)
        {
            Task.Run(() =>
            {
                Utilities.ReValidatePayPad();
            });

        }

        private async void ConfiguratePublish()
        {
            try
            {
                if (_imageSleader == null)
                {
                    _imageSleader = new ImageSleader(Utilities.PublicityPath);

                    this.DataContext = _imageSleader.imageModel;

                    _imageSleader.time = 3;

                    _imageSleader.isRotate = true;

                    _imageSleader.Start();
                }

                //WCFServices41 a = new WCFServices41();

                //WCFServices41.ConsultResolution(null);
            }
            catch (Exception ex)
            {
                //Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void LoadData()
        {
            try
            {
                string dataXml = string.Empty;
                var response = WCFServices41.DownloadData();

                if (!response.IsSuccess)
                {
                    frmLoading.Close();
                    Utilities.ShowModal(response.Message);
                    return;
                }

                Utilities.SaveFileXML(response.Result.ToString());
                dataXml = response.Result.ToString();

                dataXml = Utilities.GetFileXML();
                Peliculas data = WCFServices41.DeserealizeXML<Peliculas>(dataXml);
                DataService41.Peliculas = data;
                Utilities.LoadData();
                frmLoading.Close();

            }
            catch (System.Exception ex)
            {
                frmLoading.Close();
                AdminPaypad.SaveErrorControl(ex.Message, "LoadData en frmCinema", EError.Aplication, ELevelError.Medium);
            }
            timerStatePay.Start();
        }

        private void ConfigBoletas_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (ValidatePayPad())
                {
                    timerStatePay.Stop();
                    gridPrincipal.IsEnabled = false;
                    _imageSleader.Stop();
                    Utilities.eTypeBuy = ETypeBuy.ConfectioneryAndCinema;
                    if (Utilities.GetConfiguration("ModalBioseguridad").Equals("1"))
                    {
                        ModalBioseguridad modal = new ModalBioseguridad();
                        modal.ShowDialog();
                    }
                    Switcher.Navigate(new UCMovies());
                }
            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "BtnConsult en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

        private void Config_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (ValidatePayPad())
                {
                    timerStatePay.Stop();
                    gridPrincipal.IsEnabled = false;
                    _imageSleader.Stop();
                    Utilities.eTypeBuy = ETypeBuy.JustConfectionery;
                    Utilities.PlateObligatory = false;
                    if (Utilities.GetConfiguration("ModalBioseguridad").Equals("1"))
                    {
                        ModalBioseguridad modal = new ModalBioseguridad();
                        modal.ShowDialog();
                    }
                    Switcher.Navigate(new UCMovies());
                }
            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "BtnConsult en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }


        private bool ValidatePayPad()
        {
            if (Utilities.dataPaypad.StateUpdate)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    frmModal modal = new frmModal("Hay una nueva versión de la aplicación, por favor no manipule ni apague el dispositivo mientras se actualiza.", true);
                    modal.ShowDialog();
                    Utilities.UpdateApp();
                });
                return false;
            }
            if (Utilities.GetConfiguration("CashPayState").Equals("1"))
            {
                if (!Utilities.dataPaypad.State)
                {
                    frmModal modal = new frmModal("Perdí la conexión, intenta en un momento por favor!", false);
                    modal.ShowDialog();
                    return false;
                }
                else if (Utilities.dataPaypad.StateAceptance && Utilities.dataPaypad.StateDispenser && string.IsNullOrEmpty(Utilities.dataPaypad.Message))
                {
                    return true;
                }
                else
                {
                    frmModal modal = new frmModal(Utilities.GetConfiguration("MensajeSinDinero"));
                    modal.ShowDialog();
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
