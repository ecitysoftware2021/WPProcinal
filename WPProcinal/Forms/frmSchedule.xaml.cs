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
            imgBackground.ImageSource = Utilities.ImageSelected;
            MovieName = Utilities.CapitalizeFirstLetter(Movie.Data.TituloOriginal);
            TxtTitle.Text = MovieName;
            
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
        #endregion

        #region Methods
        //Método para generar funciones para las películas
        public void GenerateFunctions()
        {
            int i = 0;
            //Se arma una lista con los datos de sólo el cinema donde se encuentra la máquina
            var Cinema = Movie.Cinemas.Cinema.Where(f => f.Id == Utilities.CinemaId).FirstOrDefault();
            int Hour = DateTime.Now.Hour;   
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
                    if (datetime >= DateTime.Today)
                    {

                        var schedules = function.Hora.OrderBy(h => h.Militar).ToList();
                        List<HoraTMP> horatmps = new List<HoraTMP>();
                        foreach (var item in schedules)
                        {
                            horatmps.Add(new HoraTMP
                            {
                                Horario = item.Horario,
                                IdFuncion = item.IdFuncion,
                                Militar = int.Parse(item.Militar),
                                Reservas = item.Reservas,
                            });
                        }

                        int count = 0;
                        foreach (var schedule in horatmps.OrderBy(t=>t.Militar).ToList())
                        {
                            int hourcurrent = int.Parse(schedule.Militar.ToString().Substring(0, 2));
                            bool flag = true;
                            if (datetime == DateTime.Today && hourcurrent < Hour)
                            {
                                flag = false;
                            }
                            
                            if (flag)
                            {
                                string[] days = function.Dia.Split(' ');
                                int MilitarHour = Convert.ToInt16(schedule.Militar);
                                lstGrid.Add(new Schedule
                                {
                                    Id = schedule.IdFuncion,
                                    Title = Utilities.CapitalizeFirstLetter(Movie.Data.TituloOriginal),
                                    FontS = FontS,
                                    Language = string.Concat(Movie.Data.Formato, "-", Movie.Data.Idioma),
                                    Gener = Movie.Data.Genero,
                                    Duration = string.Concat(Movie.Data.Duracion, " minutos"),
                                    Category = Movie.Data.Censura,
                                    Date = string.Concat(days[1], " ", days[2], " ", days[3]),
                                    Room = string.Concat("Sala ", room.NumeroSala),
                                    Hour = schedule.Horario,
                                    RoomId = Convert.ToInt16(room.NumeroSala),
                                    UnivDate = function.Univ,
                                    MilitarHour = MilitarHour,
                                    MovieId = Convert.ToInt16(Movie.Id),
                                    TypeZona = function.TipoZona[count],
                                });
                                i++;
                            }

                            count++;
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
                return;
            }
        }

        private void CreatePages(int i)
        {
            int itemcount = i;
            //Calcular el total de páginas que tendrá la vista
            totalPage = itemcount / itemPerPage;
            if (itemcount % itemPerPage != 0)
            {
                totalPage += 1;
            }

            //Cuando sólo haya una página se ocultaran los botónes de Next y Prev
            if (totalPage == 1)
            {
                btnNext.Visibility = Visibility.Hidden;
                btnPrev.Visibility = Visibility.Hidden;
            }

            view.Source = lstGrid;
            view.Filter += new FilterEventHandler(View_Filter);
            lvSchedule.DataContext = view;
            ShowCurrentPageIndex();
            tbTotalPage.Text = totalPage.ToString();
            ValidateImage();
        }

        private void ShowCurrentPageIndex()
        {
            ValidateImage();
            tbCurrentPage.Text = (currentPageIndex + 1).ToString();
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
            Schedule schedule = (Schedule)lvSchedule.SelectedItem;
            DipMap dipmap = SetProperties(schedule);


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
