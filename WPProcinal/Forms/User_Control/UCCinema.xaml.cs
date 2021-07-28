using Grabador.Transaccion;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using WPProcinal.Classes;
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
            DataService41._Combos = new System.Collections.Generic.List<Combos>();
            timerStatePay.Interval = 10000;
            timerStatePay.Elapsed += new System.Timers.ElapsedEventHandler(TimerStatePay_Tick);

            if (Utilities.LossConnection)
            {
                Utilities.RestartApp();
            }

            try
            {
                var pat = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Renotificar", "RenotifyConsole.exe");
                Process.Start(pat);
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCCinema>UCCinema", JsonConvert.SerializeObject(ex), 1);
            }
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
        }

        private void ConfiguratePublish()
        {
            
        }


        private void ConfigBoletas_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                timerStatePay.Stop();
                gridPrincipal.IsEnabled = false;
                _imageSleader.Stop();
                Utilities.eTypeBuy = ETypeBuy.JustConfectionery;
                if (Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.ModalBioseguridad)
                {
                    ModalBioseguridad modal = new ModalBioseguridad();
                    modal.ShowDialog();
                }
                Switcher.Navigate(new UCMovies());
            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "BtnConsult en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

        private void Config_TouchDown(object sender, TouchEventArgs e)
        {
            
        }

        private void GifLoad2_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
           
        }

        private void GifLoad2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                timerStatePay.Stop();
               
                Utilities.eTypeBuy = ETypeBuy.JustConfectionery;
                Utilities.PlateObligatory = false;
               
                Switcher.Navigate(new UCMovies());
            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "BtnConsult en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }
    }
}
