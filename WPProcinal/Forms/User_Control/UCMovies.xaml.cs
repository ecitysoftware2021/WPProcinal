using System;
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

        TimerTiempo timer;

        #endregion
        public UCMovies()
        {
            InitializeComponent();
            Task.Run(() =>
            {
                ValidatePayPad();
            });
            var frmLoading = new FrmLoading("¡Cargando peliculas...!");
            frmLoading.Show();
            try
            {
                Utilities.Movies.Clear();

                Utilities.LstMovies.Clear();
                Utilities.CinemaId = Utilities.GetConfiguration("CodCinema");
                lblCinema1.Text = Dictionaries.Cinemas[Utilities.CinemaId];

                this.Dispatcher.Invoke(new ThreadStart(() =>
                {
                    DownloadData(Utilities.Peliculas);
                    CreatePages();
                    frmLoading.Close();
                }));
                Utilities.DoEvents();

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
                foreach (var pelicula in data.Pelicula)
                {
                    try
                    {
                        foreach (var Cinema in pelicula.Cinemas)
                        {
                            if (Cinema.Cinema.Id == Utilities.CinemaId)
                            {
                                var peliculaExistente = Utilities.Movies.Where(pe => pe.Data.TituloOriginal == pelicula.Data.TituloOriginal).Count();
                                if (peliculaExistente == 0)
                                {
                                    Utilities.Movies.Add(pelicula);
                                    LoadMovies(pelicula);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

        }

        public void LoadMovies(Pelicula pelicula)
        {
            try
            {
                string TagPath = string.Empty;
                //var status = WCFServices41.StateImage(pelicula.Data.Imagen);
                var movieType = GetTypeImage(pelicula.Tipo);

                string image = pelicula.Data.Imagen;
                //if (!status)
                //{
                // //   image = Path.Combine(Directory.GetCurrentDirectory(), "Images", "NotFound.jpg");
                //}
                Utilities.LstMovies.Add(new MoviesViewModel
                {
                    ImageData = Utilities.LoadImage(image, true),
                    Tag = pelicula.Id,
                    Id = pelicula.Id,
                    ImageMovieType = movieType,
                });
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "LoadMovies en frmMovies", EError.Aplication, ELevelError.Medium);
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
        private async void ValidatePayPad()
        {
            try
            {
                AdminPaypad adminPaypad = new AdminPaypad();
                adminPaypad.UpdatePeripherals();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ValidatePayPad en frmMovies", EError.Aplication, ELevelError.Medium);
            }
        }

        private void CreatePages()
        {
            try
            {
                int itemcount = Utilities.Movies.Count;

                // Calculate the total pages
                totalPage = itemcount / itemPerPage;
                if (itemcount % itemPerPage != 0)
                {
                    totalPage += 1;
                }

                if (totalPage == 1)
                {
                    btnNext.Visibility = Visibility.Hidden;
                    btnPrev.Visibility = Visibility.Hidden;
                }

                Thread.Sleep(1000);
                view.Source = Utilities.LstMovies;
                view.Filter += new FilterEventHandler(View_Filter);
                this.DataContext = view;
                ShowCurrentPageIndex();
                tbTotalPage.Text = totalPage.ToString();

                GifLoadder.Visibility = Visibility.Hidden;
                ValidateImage();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "CreatePages en frmMovies", EError.Aplication, ELevelError.Medium);
            }

        }

        private void ShowCurrentPageIndex()
        {
            ValidateImage();
            this.tbCurrentPage.Text = (currentPageIndex + 1).ToString();
        }

        private void View_Filter(object sender, FilterEventArgs e)
        {
            int index = Utilities.LstMovies.IndexOf((MoviesViewModel)e.Item);
            if (index >= itemPerPage * currentPageIndex && index < itemPerPage * (currentPageIndex + 1))
            {
                e.Accepted = true;
            }
            else
            {

                e.Accepted = false;
            }
        }
        #endregion


        #region Buttons-Events

        private void ValidateImage()
        {
            if (currentPageIndex == 0)
            {
                btnPrev.Visibility = Visibility.Hidden;
            }
            else
            {
                btnPrev.Visibility = Visibility.Visible;
            }

            if (currentPageIndex == totalPage - 1)
            {
                btnNext.Visibility = Visibility.Hidden;
            }
            else
            {
                btnNext.Visibility = Visibility.Visible;
            }
        }

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

            if (Utilities.dataPaypad.StateAceptance && Utilities.dataPaypad.StateDispenser && string.IsNullOrEmpty(Utilities.dataPaypad.Message))
            {
                Image TocuhImage = (Image)sender;
                var movie = Utilities.Movies.Where(m => m.Id == TocuhImage.Tag.ToString()).FirstOrDefault();
                //var status = WCFServices41.StateImage(movie.Data.Imagen);
                string image = movie.Data.Imagen;
                //if (!status)
                //{
                //    image = Path.Combine(Directory.GetCurrentDirectory(), "Images", "NotFound.jpg");
                //}
                Utilities.ImageSelected = Utilities.LoadImage(image, true);

                Switcher.Navigate(new UCSchedule(movie));
            }
            else
            {
                frmModal modal = new frmModal(Utilities.GetConfiguration("MensajeSinDinero"));
                modal.ShowDialog();
                Switcher.Navigate(new UCCinema());
            }
        }

        private void BtnPrev_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            ActivateTimer();
            // Display previous page
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                view.View.Refresh();
            }

            ShowCurrentPageIndex();
        }

        private void BtnNext_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            ActivateTimer();
            // Display next page
            if (currentPageIndex < totalPage - 1)
            {
                currentPageIndex++;
                view.View.Refresh();
            }
            ShowCurrentPageIndex();
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
                    Utilities.Speack("Selecciona una película para continuar.");
                    
                    this.Opacity = 1;
                    ActivateTimer();

                    if (modalCineFan.DialogResult.HasValue &&
                    modalCineFan.DialogResult.Value)
                    {
                        if (!string.IsNullOrEmpty(Utilities.dataUser.Nombre))
                        {
                            txtNameUser.Text = "Bienvenid@ " + Utilities.dataUser.Nombre.ToUpperInvariant();
                            txtNameUser.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        //Utilities.GoToInicial();
                    }
                });
            });
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
