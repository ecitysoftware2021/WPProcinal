using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        int currentPageIndex = 0;
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
                LstMoviesModel = new ObservableCollection<MoviesViewModel>();
                lblCinema1.Text = Dictionaries.Cinemas[Utilities.CinemaId];

                this.Dispatcher.Invoke(new ThreadStart(() =>
                {
                    CreatePages();
                    DownloadData(DataService41.Peliculas);
                    frmLoading.Close();
                }));
                //Utilities.DoEvents();

            }
            catch (Exception ex)
            {
                Utilities.ShowModal(ex.Message);
            }
        }

        private void DownloadData(Peliculas data)
        {
            if (data != null)
            {
                this.IsEnabled = false;
                foreach (var pelicula in data.Pelicula)
                {
                    try
                    {
                        foreach (var Cinema in pelicula.Cinemas)
                        {
                            if (Cinema.Cinema.Id == Utilities.CinemaId)
                            {
                                var peliculaExistente = DataService41.Movies.Where(pe => pe.Data.TituloOriginal == pelicula.Data.TituloOriginal).Count();
                                if (peliculaExistente == 0)
                                {
                                    DataService41.Movies.Add(pelicula);
                                    LoadMovies(pelicula);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                this.IsEnabled = true;
                Task.Run(() =>
                {
                    ValidateImages();
                });
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

                    if (BadImages.Count > 0)
                    {
                        SendMailImages();
                    }
                }
                catch (Exception)
                {

                }
                ImgValidatorFlag = true;
            }
        }

        private void SendMailImages()
        {
            try
            {
                ApiLocal api = new ApiLocal();
                Email mail;
                string data = string.Empty;

                foreach (var item in BadImages)
                {
                    data += $"Película: {item.Pelicula}, Imágen: {item.Url} <br>";
                }

                mail = new Email
                {
                    Body = $"No fué posible descargar las siguientes imágenes:<br> {data} <br>" +
                            $"Por favor revisar el repositorio de imagenes Url: {Utilities.GetConfiguration("UrlImages")} <br>" +
                            "Nota: revisar que el nombre de la imágen este bien escrito o que la imágen si exista.",
                    Subject = "Alerta Información Pay+",
                    paypad_id = Utilities.CorrespondentId
                };

                var response = api.GetResponse(mail, "SendEmail");
            }
            catch (Exception ex)
            {
            }
        }

        public void LoadMovies(Pelicula pelicula)
        {
            try
            {
                string TagPath = string.Empty;
                var movieType = GetTypeImage(pelicula.Tipo);

                string image = pelicula.Data.Imagen;

                LstMoviesModel.Add(new MoviesViewModel
                {
                    ImageData = Utilities.LoadImage(image, true),
                    Tag = pelicula.Id,
                    Id = pelicula.Id,
                    ImageMovieType = movieType,
                    Nombre = pelicula.Nombre
                });
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "LoadMovies en frmMovies", EError.Aplication, ELevelError.Medium);
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
                //view.Filter += new FilterEventHandler(View_Filter);
                this.DataContext = view;
                //ShowCurrentPageIndex();
                tbTotalPage.Text = totalPage.ToString();

                GifLoadder.Visibility = Visibility.Hidden;
                //ValidateImage();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "CreatePages en frmMovies", EError.Aplication, ELevelError.Medium);
            }

        }

        BitmapImage GetTypeImage(string type)
        {
            string image = string.Empty;
            if (type.Equals("Estreno"))
            {
                image = Path.Combine(Directory.GetCurrentDirectory(), "Images", "estrenos.png");
            }
            else if (type.Equals("Próximo Estreno"))
            {
                image = Path.Combine(Directory.GetCurrentDirectory(), "Images", "pre-ventas.png");
            }
            else
            {
                image = Path.Combine(Directory.GetCurrentDirectory(), "Images", "carteleras.png");
            }
            return new BitmapImage(new Uri(image));
        }

        void ActivateTimer()
        {
            try
            {
                tbTimer.Text = Utilities.GetConfiguration("TimerMovies");
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
            catch { }
        }

        void SetCallBacksNull()
        {
            timer.CallBackClose = null;
            timer.CallBackTimer = null;
        }

        #region Methods

        //private void View_Filter(object sender, FilterEventArgs e)
        //{
        //    int index = LstMovies.IndexOf((MoviesViewModel)e.Item);
        //    if (index >= itemPerPage * currentPageIndex && index < itemPerPage * (currentPageIndex + 1))
        //    {
        //        e.Accepted = true;
        //    }
        //    else
        //    {

        //        e.Accepted = false;
        //    }
        //}
        #endregion

        private void btnAtras_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
            }
            catch { }
            Switcher.Navigate(new UCCinema());
        }

        private void TouchImage_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
            }
            catch { }

            Image TocuhImage = (Image)sender;
            var movie = DataService41.Movies.Where(m => m.Id == TocuhImage.Tag.ToString()).FirstOrDefault();
            string image = movie.Data.Imagen;
            Utilities.ImageSelected = Utilities.LoadImage(image, true);
            Switcher.Navigate(new UCSchedule(movie));

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Dispatcher.BeginInvoke((Action)delegate
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
                            if (!string.IsNullOrEmpty(DataService41.dataUser.Nombre))
                            {
                                txtNameUser.Text = "Bienvenid@ " + DataService41.dataUser.Nombre.ToUpperInvariant();
                                txtNameUser.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            //Utilities.GoToInicial();
                        }
                    }
                    else
                    {
                        if (GetCombo())
                        {
                            Switcher.Navigate(new UCProductsCombos(new List<ChairsInformation>(), new FunctionInformation()));
                        }
                        else
                        {
                            frmModal modal = new frmModal("No se pudo descargar la confiteria, intenta de nuevo por favor!", false);
                            modal.ShowDialog();
                            Switcher.Navigate(new UCCinema());
                        }
                    }
                });
            });
        }
        private bool GetCombo()
        {
            FrmLoading frmLoading = new FrmLoading("¡Descargando la confitería...!");
            frmLoading.Show();
            var combos = WCFServices41.GetConfectionery(new SCOPRE
            {
                teatro = Utilities.GetConfiguration("CodCinema"),
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
                    timer.CallBackStop?.Invoke(1);

                    this.Opacity = 0.3;
                    frmModalLogin modalLogin = new frmModalLogin();
                    modalLogin.ShowDialog();

                    if (modalLogin.DialogResult.HasValue && modalLogin.DialogResult.Value)
                    {
                        FrmModalDeletePay modalDeletePay = new FrmModalDeletePay();
                        modalDeletePay.ShowDialog();
                    }

                    this.Opacity = 1;
                    ActivateTimer();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
