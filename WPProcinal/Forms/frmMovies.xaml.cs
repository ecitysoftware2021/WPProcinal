using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WPProcinal.Classes;
using WPProcinal.Models;

namespace WPProcinal.Forms
{
    public partial class frmMovies : Window
    {
        #region Reference
        CollectionViewSource view = new CollectionViewSource();
        int currentPageIndex = 0;
        int itemPerPage = 6;
        int totalPage = 0;

        TimerTiempo timer;

        #endregion

        #region LoadMethods
        public frmMovies()
        {

            InitializeComponent();

            Task.Run(() =>
            {
                ValidatePayPad();
            });
            var frmLoading = new FrmLoading("¡Cargando peliculas!");
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

                ActivateTimer();
            }
            catch (Exception ex)
            {
                Utilities.ShowModal(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void DownloadData(Peliculas data)
        {
            try
            {
                if (data != null)
                {
                    foreach (var pelicula in data.Pelicula)
                    {
                        foreach (var Cinema in pelicula.Cinemas.Cinema)
                        {
                            if (Cinema.Id == Utilities.CinemaId)
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
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "DownloadData en frmMovies", EError.Aplication, ELevelError.Medium);
            }
        }

        public void LoadMovies(Pelicula pelicula)
        {
            try
            {
                string ImagePath = string.Concat(Utilities.UrlImages, pelicula.Id, ".jpg");
                string TagPath = string.Empty;

                Utilities.LstMovies.Add(new MoviesViewModel
                {
                    ImageData = Utilities.LoadImage(ImagePath, true),
                    Tag = pelicula.Id,
                    Id = pelicula.Id,
                });

            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "LoadMovies en frmMovies", EError.Aplication, ELevelError.Medium);
            }
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
                        frmCinema main = new frmCinema();
                        main.Show();
                        this.Close();
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


        #endregion

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
                lvMovies.DataContext = view;
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
                btnAtras.IsEnabled = false;
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
            }
            catch { }
            //Utilities.ResetTimer();
            frmCinema frmCinema = new frmCinema();
            frmCinema.Show();
            Close();
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
            }
            catch { }

            if (Utilities.dataPaypad.StateAceptance && Utilities.dataPaypad.StateDispenser && string.IsNullOrEmpty(Utilities.dataPaypad.Message))
            {
                Grid grid = (Grid)sender;
                var movie = Utilities.Movies.Where(m => m.Id == grid.Tag.ToString()).FirstOrDefault();
                string ImagePath = string.Concat(Utilities.UrlImages, movie.Id, ".jpg");
                Utilities.ImageSelected = Utilities.LoadImage(ImagePath, true);

                frmSchedule frmSchedule = new frmSchedule(movie);
                frmSchedule.Show();
                Close();
            }
            else
            {
                frmModal modal = new frmModal(Utilities.GetConfiguration("MensajeSinDinero"));
                modal.ShowDialog();
                frmCinema frmCinema = new frmCinema();
                frmCinema.Show();
                Close();
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
    }
}
