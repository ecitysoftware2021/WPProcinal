using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPProcinal.Classes;
using WPProcinal.Models;
using WPProcinal.Service;

namespace WPProcinal.Forms.User_Control
{
    /// <summary>
    /// Interaction logic for UCMovies.xaml
    /// </summary>
    public partial class UCMovies : UserControl
    {
        #region Reference
        CollectionViewSource view = new CollectionViewSource();
        int itemPerPage = 20;
        int totalPage = 0;
        int login = 0;

        /// <summary>
        /// Variable de control, para evitar validar varias veces la misma imagen,
        /// esta validacion de imagen se hace para notificar al cinema cuando un poster esta malo
        /// </summary>
        private bool ImgValidatorFlag = true;

        /// <summary>
        /// Lista de imagenes que estan mal cargadas, esta lista se reporta por correo al cinema para su corrección.
        /// </summary>
        private List<DataImagesScore> BadImages;

        /// <summary>
        /// Modelo que se va a bindar para mostrar la lista de peliculas
        /// </summary>
        private ObservableCollection<MoviesViewModel> LstMoviesModel;

        TimerTiempo timer;

        #endregion

        /// <summary>
        /// TypeBuy 2: Boletas & Confiteria, 1: Solo Confiteria
        /// </summary>
        /// <param name="typeBuy"></param>
        public UCMovies()
        {
            InitializeComponent();

            var frmLoading = new FrmLoading("¡Cargando peliculas...!");
            frmLoading.Show();

            try
            {
                DataService41.Movies = new List<Pelicula>();
                Utilities.dataTransaction = new DataTransaction();
                LstMoviesModel = new ObservableCollection<MoviesViewModel>();
                lblCinema1.Text = Dictionaries.Cinemas[Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema.ToString()];

                this.Dispatcher.Invoke(new ThreadStart(() =>
                {
                    CreatePages();
                    DownloadData(DataService41.Peliculas);
                    frmLoading.Close();
                }));

            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCMovies>UCMovies", JsonConvert.SerializeObject(ex), 1);
                Utilities.ShowModal(ex.Message);
            }
        }

        private void DownloadData(Peliculas data)
        {
            if (data != null)
            {
                this.IsEnabled = false;
                foreach (var pelicula in data.Pelicula.Where(c=>c.Cinemas !=null))
                {
                    try
                    {
                        //if (pelicula.Cinemas != null)
                        //{
                            foreach (var Cinema in pelicula.Cinemas.Cinema.Where(c=>c.Id == Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema.ToString()))
                            {
                                if (Cinema.Id == Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema.ToString())
                                {
                                    var peliculaExistente = DataService41.Movies.Where(pe => pe.Data.TituloOriginal == pelicula.Data.TituloOriginal).Count();
                                    if (peliculaExistente == 0)
                                    {
                                        DataService41.Movies.Add(pelicula);
                                        LoadMovies(pelicula);
                                    }
                                }
                            }
                        //}
                    }
                    catch (Exception ex)
                    {
                        LogService.SaveRequestResponse("UCMovies>DownloadData", JsonConvert.SerializeObject(ex), 1);
                    }
                }
                this.IsEnabled = true;
                
                //Task.Run(() =>
                //{
                //    ValidateImages();
                //});
            }
        }

        private void ValidateImages()
        {
            if (ImgValidatorFlag)
            {
                ImgValidatorFlag = false;
                try
                {
                    BadImages = new List<DataImagesScore>();
                    ObservableCollection<MoviesViewModel> images = new ObservableCollection<MoviesViewModel>();
                    foreach (var item in LstMoviesModel)
                    {
                        images.Add(item);
                    }
                    foreach (var item in images)
                    {
                        if (!WCFServices41.StateImage(item.ImageTag))
                        {
                            BadImages.Add(new DataImagesScore
                            {
                                Pelicula = item.Title,
                                Url = item.ImageTag
                            });
                        }
                    }
                }
                
                catch (Exception ex)
                {
                    LogService.SaveRequestResponse("UCMovies>ValidateImages", JsonConvert.SerializeObject(ex), 1);
                }
                ImgValidatorFlag = true;
            }
        }

        public void LoadMovies(Pelicula pelicula)
        {
            try
            {
                string TagPath = string.Empty;
                var movieType = GetTypeImage(pelicula.Tipo);

                string image = pelicula.Data.Imagen;
                
                if (!File.Exists(Path.Combine(Path.GetDirectoryName(
                    Assembly.GetEntryAssembly().Location), "Imagesmovies", pelicula.Data.TituloOriginal + ".png")))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFileAsync(new Uri(image), Path.Combine(Path.GetDirectoryName(
                         Assembly.GetEntryAssembly().Location),
                         "Imagesmovies", pelicula.Data.TituloOriginal.Trim() + ".png"));
                    }
                }

                LstMoviesModel.Add(new MoviesViewModel
                {
                    ImageData =
                    Utilities.LoadImage(Path.Combine(Path.GetDirectoryName(
                         Assembly.GetEntryAssembly().Location),
                         "Imagesmovies", pelicula.Data.TituloOriginal + ".png"), true),
                    //Utilities.LoadImage(image, true),
                    Tag = pelicula.Id,
                    Id = pelicula.Id,
                    ImageMovieType = movieType,
                    Nombre = pelicula.Data.TituloOriginal
                });
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "LoadMovies en frmMovies", EError.Aplication, ELevelError.Medium);
            }
        }

        private void CreatePages()
        {
            try
            {
                int itemcount = DataService41.Movies.Count;

                // Calculate the total pages
                totalPage = itemcount / itemPerPage;
                if (itemcount % itemPerPage != 0)
                {
                    totalPage += 1;
                }


                Thread.Sleep(1000);
                view.Source = LstMoviesModel;
                this.DataContext = view;
                tbTotalPage.Text = totalPage.ToString();
                GifLoadder.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(JsonConvert.SerializeObject(ex), "CreatePages en frmMovies", EError.Aplication, ELevelError.Medium);
            }

        }

        BitmapImage GetTypeImage(string type)
        {
            string image = string.Empty;

            if (type.Equals("Estreno"))
            {
                string pat = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "estrenos.png");
                image = pat;
            }
            else if (type.Equals("Próximo Estreno"))
            {
                string pat = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "pre-ventas.png");
                image = pat;
            }
            else
            {
                string pat = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "carteleras.png");
                image = pat;
            }
            return new BitmapImage(new Uri(image));
        }

        void ActivateTimer()
        {
            try
            {
                tbTimer.Text = Utilities.dataPaypad.PaypadConfiguration.generiC_TIMER;
                timer = new TimerTiempo(tbTimer.Text);
                timer.CallBackClose = response =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Switcher.Navigate(new UCCinema());
                    });
                };
                timer.CallBackTimer = response =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        tbTimer.Text = response;
                    });
                };
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCMovies>ActivateTimer", JsonConvert.SerializeObject(ex), 1);
            }
        }

        void SetCallBacksNull()
        {
            if (timer != null)
            {
                timer.CallBackClose = null;
                timer.CallBackTimer = null;
                timer.CallBackStop?.Invoke(1);

            }
        }


        private void btnAtras_TouchDown(object sender, TouchEventArgs e)
        {
            this.IsEnabled = false;
            SetCallBacksNull();
            Switcher.Navigate(new UCCinema());
        }

        private void TouchImage_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            try
            {
                Image TocuhImage = (Image)sender;
                var movie = DataService41.Movies.Where(m => m.Id == TocuhImage.Tag.ToString()).FirstOrDefault();
                string image = movie.Data.Imagen;
                Utilities.dataTransaction.ImageSelected = Utilities.LoadImage(image, true);
                Switcher.Navigate(new UCSchedule(movie));
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCMovies>TouchImage_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            this.Opacity = 0.3;
            frmModalCineFan modalCineFan = new frmModalCineFan();
            modalCineFan.ShowDialog();
            if (Utilities.eTypeBuy == ETypeBuy.ConfectioneryAndCinema)
            {
                Utilities.Speack("Selecciona una película para continuar.");

                this.Opacity = 1;
                ActivateTimer();

                if (modalCineFan.DialogResult.HasValue &&
                modalCineFan.DialogResult.Value)
                {
                    try
                    {
                        if (Utilities.dataTransaction != null && Utilities.dataTransaction.dataUser != null)
                        {
                            if (!string.IsNullOrEmpty(Utilities.dataTransaction.dataUser.Nombre))
                            {
                                txtNameUser.Text = "Bienvenid@ " + Utilities.dataTransaction.dataUser.Nombre.ToUpperInvariant();
                                txtNameUser.Visibility = Visibility.Visible;
                            }
                            else if (!string.IsNullOrEmpty(Utilities.dataTransaction.dataDocument.FirstName))
                            {
                                txtNameUser.Text = "Bienvenid@ " + Utilities.dataTransaction.dataDocument.FirstName.ToUpperInvariant();
                                txtNameUser.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.SaveRequestResponse("UCMovies>UserControl_Loaded", JsonConvert.SerializeObject(ex), 1);
                    }
                }
            }
            else
            {
                if (GetCombo())
                {
                    Switcher.Navigate(new UCProductsCombos());
                }
                else
                {
                    frmModal modal = new frmModal("No se pudo descargar la confiteria, intenta de nuevo por favor!", false);
                    modal.ShowDialog();
                    Switcher.Navigate(new UCCinema());
                }
            }
        }
        private bool GetCombo()
        {
            FrmLoading frmLoading = new FrmLoading("¡Descargando la confitería...!");
            frmLoading.Show();
            var combos = WCFServices41.GetConfectionery(new SCOPRE
            {
                teatro = Utilities.dataPaypad.PaypadConfiguration.ExtrA_DATA.CodCinema.ToString(),
                tercero = "1"
            });
            frmLoading.Close();
            if (combos != null)
            {
                if (combos.ListaProductos.Count > 0)
                {
                    DataService41._Productos = combos.ListaProductos;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void BtnLogin_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                login += 1;

                if (login == 5)
                {
                    SetCallBacksNull();

                    this.Opacity = 0.3;
                    frmModalLogin modalLogin = new frmModalLogin();
                    modalLogin.ShowDialog();

                    if (modalLogin.DialogResult.HasValue && modalLogin.DialogResult.Value)
                    {
                        FrmModalDeletePay modalDeletePay = new FrmModalDeletePay();
                        modalDeletePay.ShowDialog();
                        login = 0;
                    }

                    this.Opacity = 1;
                    ActivateTimer();
                }
            }
            catch (Exception ex)
            {
                LogService.SaveRequestResponse("UCMovies>BtnLogin_TouchDown", JsonConvert.SerializeObject(ex), 1);
            }
        }
    }
}
