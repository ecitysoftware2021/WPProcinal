using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPProcinal.Classes;
using WPProcinal.Models;

namespace WPProcinal.Forms
{
    public partial class frmSchedule : Window
    {
        #region References
        string MovieName = string.Empty;
        CollectionViewSource view = new CollectionViewSource();
        ObservableCollection<Schedule> lstGrid = new ObservableCollection<Schedule>();
        int currentPageIndex = 0;
        int itemPerPage = 6;
        int totalPage = 0;
        int FontS = 0;
        int cinemaId = Convert.ToInt16(Utilities.GetConfiguration("CodCinema"));
        Pelicula Movie = new Pelicula();
        #endregion

        #region LoadMethods
        public frmSchedule(Pelicula Movie)
        {
            InitializeComponent();
            this.Movie = Movie;
            Utilities.Movie = Movie;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer, this);

            imgBackground.ImageSource = Utilities.ImageSelected;
            MovieName = Utilities.CapitalizeFirstLetter(Movie.Data.TituloOriginal);
            TxtTitle.Text = MovieName;

            DateTime fechaActual = DateTime.Today;

            TxtDay.Text = string.Format("{0} {1}, {2}", fechaActual.ToString("dddd"), fechaActual.Day, fechaActual.ToString("MMM"));

            if (Movie.Data.TituloOriginal.Length <= 15)
            {
                FontS = 55;
            }
            else
            {
                FontS = 35;
            }

            GenerateFunctions();
        }

        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);


        #endregion

        #region Methods
        //Método para generar funciones para las películas
        public void GenerateFunctions()
        {
            try
            {
                int i = 0;
                //Se arma una lista con los datos de sólo el cinema donde se encuentra la máquina
                //var Cinema = Movie.Cinemas.Cinema.Where(f => f.Id == Utilities.CinemaId).FirstOrDefault();
                int Hour = DateTime.Now.Hour;

                foreach (var peli in Utilities.Peliculas.Pelicula.Where(pe => pe.Data.TituloOriginal == Movie.Data.TituloOriginal))
                {
                    foreach (var Cinema in peli.Cinemas.Cinema.Where(cine => cine.Id == Utilities.CinemaId))
                    {
                        //Se recorre cada sala del Cinema
                        foreach (var room in Cinema.Salas.Sala)
                        {
                            //Se arma una lista con las fechas de cada función y luego se recorre
                            var functions = room.Fecha.ToList();
                            foreach (var function in functions)
                            {
                                /*Se obtiene la fecha de la función  y luego se valida para no mostrar
                                funciones con fechas menores a la actual*/
                                var datetime = GetDateCorrectly(function.Univ);
                                //TODO:fechas mayores a hoy, version anterior
                                //if (datetime >= DateTime.Today)
                                if (datetime == DateTime.Today)
                                {

                                    var schedules = function.Hora.OrderBy(h => h.Militar).ToList();
                                    List<HoraTMP> horatmps = new List<HoraTMP>();
                                    int count = 0;
                                    int countAvailable = 0;
                                    bool available = true;
                                    foreach (var item in schedules)
                                    {
                                        if (int.Parse(item.Militar) > int.Parse(DateTime.Now.ToString("HHmm")))
                                        {
                                            horatmps.Add(new HoraTMP
                                            {
                                                Horario = item.Horario,
                                                IdFuncion = item.IdFuncion,
                                                Militar = int.Parse(item.Militar),
                                                Reservas = item.Reservas,
                                                TipoZona = function.TipoZona[count]
                                            });
                                        }
                                        else
                                        {
                                            countAvailable++;
                                        }
                                        if (countAvailable == schedules.Count())
                                        {
                                            available = false;
                                        }
                                        count++;
                                    }


                                    if (available)
                                    {
                                        string ruta = GenerateTags(room.TipoSala);

                                        string[] days = function.Dia.Split(' ');
                                        //int MilitarHour = Convert.ToInt16(schedule.Militar);
                                        lstGrid.Add(new Schedule
                                        {
                                            //Id = schedule.IdFuncion,
                                            Title = Utilities.CapitalizeFirstLetter(Movie.Data.TituloOriginal),
                                            FontS = FontS,
                                            Language = string.Concat(Movie.Data.Idioma),
                                            Gener = Movie.Data.Genero,
                                            Duration = string.Concat(Movie.Data.Duracion, " minutos"),
                                            Category = Movie.Data.Censura,
                                            Date = string.Concat(days[1], " ", days[2], " ", days[3]),
                                            Room = string.Concat("Sala ", room.NumeroSala),
                                            //Hour = schedule.Horario,
                                            Hours = horatmps,
                                            TipoSala = ruta,
                                            RoomId = Convert.ToInt16(room.NumeroSala),
                                            UnivDate = function.Univ,
                                            //MilitarHour = MilitarHour,
                                            MovieId = Convert.ToInt16(peli.Id),
                                            //TypeZona = function.TipoZona[count],
                                            Formato = peli.Data.Formato
                                        });
                                        foreach (var newHoras in horatmps)
                                        {
                                            newHoras.DatosPelicula = new Schedule
                                            {
                                                Title = Utilities.CapitalizeFirstLetter(Movie.Data.TituloOriginal),
                                                FontS = FontS,
                                                Language = string.Concat(Movie.Data.Idioma),
                                                Gener = Movie.Data.Genero,
                                                Duration = string.Concat(Movie.Data.Duracion, " minutos"),
                                                Category = Movie.Data.Censura,
                                                Date = string.Concat(days[1], " ", days[2], " ", days[3]),
                                                Room = string.Concat("Sala ", room.NumeroSala),
                                                Hour = newHoras.Horario,
                                                //Hours = horatmps,
                                                TipoSala = ruta,
                                                RoomId = Convert.ToInt16(room.NumeroSala),
                                                UnivDate = function.Univ,
                                                //MilitarHour = MilitarHour,
                                                MovieId = Convert.ToInt16(peli.Id),
                                                //TypeZona = function.TipoZona[count],
                                                Formato = peli.Data.Formato
                                            };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (lstGrid.Count > 0)
                {
                    CreatePages(i);
                }
                else
                {
                    Utilities.ShowModal("Lo sentimos, no hay funciones para esta película");
                    Utilities.GoToInicial(this);
                    return;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private string GenerateTags(string name)
        {
            string Path = string.Empty;
            switch (name)
            {
                case "4DX":
                    return "/Images/Tags/cartel-4dx.png";
                case "2D":
                    return "/Images/Tags/cartel-2d.png";
                case "Vibrasound":
                    return "/Images/Tags/cartel-vibra.png";
                case "Supernova":
                    return "/Images/Tags/cartel-supernova.png";
                case "Star Kids":
                    return "/Images/Tags/cartel-starkids.png";
                case "Cine Arte":
                    return "/Images/Tags/cartel-cinearte.png";
                case "Black Star":
                    return "/Images/Tags/cartel-blackstar.png";
                default:
                    return string.Empty;
            }
        }


        private void CreatePages(int i)
        {
            //int itemcount = i;
            ////Calcular el total de páginas que tendrá la vista
            //totalPage = itemcount / itemPerPage;
            //if (itemcount % itemPerPage != 0)
            //{
            //    totalPage += 1;
            //}

            ////Cuando sólo haya una página se ocultaran los botónes de Next y Prev
            //if (totalPage == 1)
            //{
            //    btnNext.Visibility = Visibility.Hidden;
            //    btnPrev.Visibility = Visibility.Hidden;
            //}

            //view.Source = lstGrid;
            //view.Filter += new FilterEventHandler(View_Filter);
            lvSchedule.DataContext = lstGrid;
            //ShowCurrentPageIndex();
            //tbTotalPage.Text = totalPage.ToString();
            //ValidateImage();
        }

        private void ShowCurrentPageIndex()
        {
            ValidateImage();
            //tbCurrentPage.Text = (currentPageIndex + 1).ToString();
        }

        private void View_Filter(object sender, FilterEventArgs e)
        {
            int index = lstGrid.IndexOf((Schedule)e.Item);

            if (index >= itemPerPage * currentPageIndex && index < itemPerPage * (currentPageIndex + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private DateTime GetDateCorrectly(string univDate)
        {
            char[] Date = univDate.ToCharArray();
            string year = univDate.Substring(0, 4);
            string month = string.Concat(Date[4], Date[5]);
            string day = string.Concat(Date[6], Date[7]);
            var dateTime = Convert.ToDateTime(string.Format("{0}/{1}/{2}", year, month, day));
            return dateTime;
        }

        private string InvertDate(string univDate)
        {
            char[] Date = univDate.ToCharArray();
            string year = univDate.Substring(0, 4);
            string month = string.Concat(Date[4], Date[5]);
            string day = string.Concat(Date[6], Date[7]);
            string DateInvert = string.Concat(day, month, year);

            return DateInvert;
        }

        private DipMap SetProperties(Schedule schedule)
        {
            int Hour = Convert.ToInt32(schedule.MilitarHour.ToString().Substring(0, 2));
            string DateFormat = InvertDate(schedule.UnivDate);
            DipMap map = new DipMap
            {
                MovieName = schedule.Title,
                Language = schedule.Language,
                Gener = schedule.Gener,
                Duration = schedule.Duration,
                HourFunction = schedule.Hour,
                RoomName = schedule.Room,
                Category = schedule.Category,
                Day = schedule.Date,
                Date = schedule.UnivDate,
                DateFormat = DateFormat,
                CinemaId = cinemaId,
                RoomId = schedule.RoomId,
                Hour = Hour,
                HourFormat = schedule.MilitarHour,
                MovieId = schedule.MovieId,
                Letter = "A",
                IsCard = "S",
                Group = 1,
                Login = "brandon-377@hotmail.com",
                PointOfSale = 87,
                PaymentMethod = "E",
                TypeZona = schedule.TypeZona,
            };

            return map;
        }
        #endregion

        #region ButtonsEvents
        private void LvSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedSchedule = (HoraTMP)(sender as ListView).SelectedValue;
            Schedule schedule = new Schedule
            {
                Category = selectedSchedule.DatosPelicula.Category,
                Date = selectedSchedule.DatosPelicula.Date,
                Duration = selectedSchedule.DatosPelicula.Duration,
                FontS = 0,
                Formato = selectedSchedule.DatosPelicula.Formato,
                Gener = selectedSchedule.DatosPelicula.Gener,
                Hour = selectedSchedule.DatosPelicula.Hour,
                Id = selectedSchedule.DatosPelicula.Id,
                Language = selectedSchedule.DatosPelicula.Language,
                MilitarHour = selectedSchedule.Militar,
                MovieId = selectedSchedule.DatosPelicula.MovieId,
                Room = selectedSchedule.DatosPelicula.Room,
                RoomId = selectedSchedule.DatosPelicula.RoomId,
                //TipoSala=,
                Title = selectedSchedule.DatosPelicula.Title,
                TypeZona = selectedSchedule.TipoZona,
                UnivDate = selectedSchedule.DatosPelicula.UnivDate
            };
            //Schedule schedule = (Schedule)lvSchedule.SelectedItem;
            DipMap dipmap = SetProperties(schedule);

            Utilities.ResetTimer();
            frmSeat frmSeat = new frmSeat(dipmap);
            frmSeat.Show();
            Close();
        }

        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            frmMovies frmMovies = new frmMovies();
            frmMovies.Show();
            Close();
        }

        private void ValidateImage()
        {
            //if (currentPageIndex == 0)
            //{
            //    btnPrev.Visibility = Visibility.Hidden;
            //}
            //else
            //{
            //    btnPrev.Visibility = Visibility.Visible;
            //}

            //if (currentPageIndex == totalPage - 1)
            //{
            //    btnNext.Visibility = Visibility.Hidden;
            //}
            //else
            //{
            //    btnNext.Visibility = Visibility.Visible;
            //}
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
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
            //ListView lv = lvSchedule;
            //DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
            //lv.BeginAnimation(OpacityProperty, animation);

            // Display next page
            if (currentPageIndex < totalPage - 1)
            {
                currentPageIndex++;
                view.View.Refresh();
            }
            ShowCurrentPageIndex();
            //lv.Visibility = Visibility.Visible;
        }
        #endregion
    }
}
