using Grabador.Transaccion;
using PrinterValidator;
using System;
using System.Diagnostics;
using System.IO;
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
        private FrmLoading frmLoading;
        private ImageSleader _imageSleader;

        public UCCinema()
        {
            InitializeComponent();
            ConfiguratePublish();
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
            frmLoading = new FrmLoading("¡Descargando información...!");


            Task.Run(() => Dispatcher.BeginInvoke((Action)delegate
            {
                frmLoading.Show();
                LoadData();
            }));

        }

        private void ConfiguratePublish()
        {
            try
            {
                if (_imageSleader == null)
                {
                    _imageSleader = new ImageSleader(Utilities.path);

                    this.DataContext = _imageSleader.imageModel;

                    _imageSleader.time = 3;

                    _imageSleader.isRotate = true;

                    _imageSleader.Start();
                }
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

                Utilities.LstMovies.Clear();

                string dataXml = string.Empty;
                var response = WCFServices41.DownloadData();
                frmLoading.Close();
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
                _imageSleader.Stop();
                Switcher.Navigate(new UCMovies());
            }
            catch (System.Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "BtnConsult en frmCinema", EError.Aplication, ELevelError.Medium);
            }
        }

    }
}
