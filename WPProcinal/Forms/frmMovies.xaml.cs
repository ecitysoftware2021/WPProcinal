using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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
        List<ImageSource> movie_posters_list = new List<ImageSource>();
        List<String> movie_names = new List<String>();
        CollectionViewSource view = new CollectionViewSource();
        int currentPageIndex = 0;
        int itemPerPage = 6;
        int totalPage = 0;
        string Cinema = string.Empty;

        TimerTiempo timer;

        /*--LIST DATE--*/
        private DateTime FechaSelect = new DateTime();
        private List<DateName> dateName = new List<DateName>();
        List<Border> borders = new List<Border>();
        private int currentPageIndex2 = 0;
        private int itemPerPage2 = 3;
        private int totalPage2 = 0;
        private CollectionViewSource view2 = new CollectionViewSource();
        private ObservableCollection<DateName> lstPager2 = new ObservableCollection<DateName>();
        List<ListViewItem> grid = new List<ListViewItem>();
        /*--END DATE--*/
        #endregion

        #region LoadMethods
        public frmMovies()
        {

            InitializeComponent();

            var frmLoading = new FrmLoading("¡Cargando peliculas!");
            frmLoading.Show();
            try
            {
                Utilities.Movies.Clear();
                Utilities.CinemaId = Utilities.GetConfiguration("CodCinema");
                lblCinema1.Text = Dictionaries.Cinemas[Utilities.CinemaId];

                this.Dispatcher.Invoke(new ThreadStart(() =>
                {
                    DownloadData(Utilities.Peliculas);
                    CreatePages();
                    frmLoading.Close();
                }));
                Utilities.DoEvents();

                //ListFechas();
                //InitView2();
                ActivateTimer();
            }
            catch (Exception ex)
            {
                Utilities.ShowModal(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Utilities.Timer(tbTimer,this);
        }

        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        private void DownloadData(Peliculas data)
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

        public void LoadMovies(Pelicula pelicula)
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

        private static string LoadImagePath(Pelicula pelicula)
        {
            string Path = GenerateTag(pelicula);
            string TagPath = string.Concat("../Images/Tags/cartel-", Path, ".png");

            return TagPath;
        }

        private static string GenerateTag(Pelicula pelicula)
        {
            string Path = string.Empty;
            string[] Name = pelicula.Nombre.Split(' ');

            if (Name[0].Contains("2D"))
            {
                if (Name[1].Contains("BS"))
                {
                    Path = "blackstar";
                }
                else if (Name[1].Contains("CA"))
                {
                    Path = "cinearte";
                }
                else if (Name[1].Contains("SK"))
                {
                    Path = "starkids";
                }
                else if (Name[1].Contains("SN"))
                {
                    Path = "supernova";
                }
                else if (Name[1].Contains("VIB"))
                {
                    Path = "2d-vibra";
                }
                else
                {
                    Path = "2d";
                }
            }
            else if (Name[0].Contains("3D"))
            {
                if (Name[1].Contains("4DX"))
                {
                    Path = "4dx";
                }
                else if (Name[1].Contains("BS"))
                {
                    Path = "blackstar";
                }
                else if (Name[1].Contains("SN"))
                {
                    Path = "supernova";
                }
                else if (Name[1].Contains("VIB"))
                {
                    Path = "3d-vibra";
                }
                else
                {
                    Path = "3d";
                }
            }

            return Path;
        }


        void ActivateTimer()
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

        void SetCallBacksNull()
        {
            timer.CallBackClose = null;
            timer.CallBackTimer = null;
        }


        #endregion

        #region Methods

        private void CreatePages()
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

            //this.Dispatcher.Invoke(() =>
            //{
            Thread.Sleep(1000);
            view.Source = Utilities.LstMovies;
            view.Filter += new FilterEventHandler(View_Filter);
            lvMovies.DataContext = view;
            ShowCurrentPageIndex();
            tbTotalPage.Text = totalPage.ToString();

            GifLoadder.Visibility = Visibility.Hidden;
            ValidateImage();
            //});

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

        //#region "ListViewDate"
        //private void ListFechas()
        //{
        //    try
        //    {
        //        for (int i = 0; i < 8; i++)
        //        {
        //            DateTime dt2 = DateTime.Now.AddDays(i);
        //            string NombreDiaAdd = dt2.ToString("dddd", CultureInfo.CreateSpecificCulture("es-ES"));

        //            if (NombreDiaAdd != "miércoles")
        //            {
        //                dateName.Add(new DateName
        //                {
        //                    FechaOrigin = dt2,
        //                    Mes = dt2.ToString("MMM"),
        //                    NombreDia = NombreDiaAdd,
        //                    DiaNumero = dt2.ToString("dd")
        //                });
        //            }
        //            else
        //            {
        //                dateName.Add(new DateName
        //                {
        //                    FechaOrigin = dt2,
        //                    Mes = dt2.ToString("MMM"),
        //                    NombreDia = NombreDiaAdd,
        //                    DiaNumero = dt2.ToString("dd")
        //                });

        //                return;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private void InitView2()
        //{
        //    try
        //    {
        //        if (dateName.Count() > 0)
        //        {
        //            foreach (var item in dateName)
        //            {
        //                lstPager2.Add(new DateName
        //                {
        //                    Mes = item.Mes,
        //                    NombreDia = item.NombreDia,
        //                    FechaOrigin = item.FechaOrigin,
        //                    TextColor = "Black",
        //                    DiaNumero = item.DiaNumero
        //                });
        //            }

        //            CreatePages2(dateName.Count());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private void CreatePages2(int i)
        //{
        //    try
        //    {
        //        int itemcount = i;
        //        totalPage2 = itemcount / itemPerPage2;
        //        if (itemcount % itemPerPage2 != 0)
        //        {
        //            totalPage2 += 1;
        //        }
        //        lstPager2[0].TextColor = "#FF1385FF";
        //        view2.Source = lstPager2;
        //        view2.Filter += new FilterEventHandler(View_Filter2);
        //        lv_DateName.DataContext = view2;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private void View_Filter2(object sender, FilterEventArgs e)
        //{
        //    try
        //    {
        //        int index = lstPager2.IndexOf((DateName)e.Item);

        //        if (index >= itemPerPage2 * currentPageIndex2 && index < itemPerPage2 * (currentPageIndex2 + 1))
        //        {
        //            e.Accepted = true;
        //        }
        //        else
        //        {
        //            e.Accepted = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private async void lv_DateName_PreviewStylusDown(object sender, StylusDownEventArgs e)
        //{
        //    try
        //    {
        //        var service = (DateName)(sender as ListViewItem).Content;

        //        FechaSelect = service.FechaOrigin;//poner la fecha en el formato xml
        //        Utilities.FechaSeleccionada = FechaSelect;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private void Grid_PreviewStylusDown(object sender, StylusDownEventArgs e)
        //{
        //    try
        //    {
        //        var childs = (sender as Grid).Children;
        //        foreach (var item in childs)
        //        {
        //            if (item is Border)
        //            {
        //                ClearHoursList();
        //                var border = item as Border;
        //                Color color2 = (Color)ColorConverter.ConvertFromString("#FFF89126");
        //                border.Background = new SolidColorBrush(color2);
        //                borders.Add(border);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private void ClearHoursList()
        //{
        //    foreach (var item in borders)
        //    {
        //        item.Background = Brushes.White;
        //    }
        //}

        //private void btnNext2_PreviewStylusDown(object sender, StylusDownEventArgs e)
        //{
        //    try
        //    {
        //        SetCallBacksNull();
        //        ActivateTimer();
        //        if (currentPageIndex2 < totalPage2 - 1)
        //        {
        //            currentPageIndex2++;
        //            view2.View.Refresh();
        //        }
        //        if (currentPageIndex2 == totalPage2 - 1)
        //        {
        //            btnNext2.Visibility = Visibility.Hidden;
        //        }

        //        btnPrev2.Visibility = Visibility.Visible;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private void btnPrev2_PreviewStylusDown(object sender, StylusDownEventArgs e)
        //{
        //    try
        //    {
        //        SetCallBacksNull();
        //        ActivateTimer();
        //        if (currentPageIndex2 > 0)
        //        {
        //            currentPageIndex2--;
        //            view2.View.Refresh();
        //        }

        //        if (currentPageIndex2 == 0)
        //        {
        //            btnPrev2.Visibility = Visibility.Hidden;
        //        }

        //        btnNext2.Visibility = Visibility.Visible;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        //#endregion

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

        private void btnPrev_Click(object sender, RoutedEventArgs e)
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

        private void btnNext_Click(object sender, RoutedEventArgs e)
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

        private void Movie_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = (Grid)sender;
            var movie = Utilities.Movies.Where(m => m.Id == grid.Tag.ToString()).FirstOrDefault();
            string ImagePath = string.Concat(Utilities.UrlImages, movie.Id, ".jpg");
            Utilities.ImageSelected = Utilities.LoadImage(ImagePath, true);


            //if (ControlPantalla.EstadoBaul && ControlPantalla.EstadoBilletes && ControlPantalla.EstadoMonedas)
            //{
            //Utilities.ResetTimer();
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            frmSchedule frmSchedule = new frmSchedule(movie);
            frmSchedule.Show();
            Close();
            //}
            //else
            //{

            //    frmModal _modal = new frmModal(string.Concat(
            //        "En estos momentos no se pueden realizar transacciones.",
            //        Environment.NewLine,
            //        "Por favor intente más tarde."));
            //    _modal.ShowDialog();

            //    frmCinema _frmCinema = new frmCinema();
            //    _frmCinema.Show();
            //    Close();
            //}
        }

        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
        #endregion
    }
}
