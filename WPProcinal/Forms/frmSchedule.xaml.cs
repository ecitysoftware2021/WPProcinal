using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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

        TimerTiempo timer;
        #endregion

        #region LoadMethods
        public frmSchedule(Pelicula Movie)
        {
            InitializeComponent();
            this.Movie = Movie;
            Utilities.Movie = Movie;

            foreach (var peli in Utilities.Peliculas.Pelicula.Where(pe => pe.Data.TituloOriginal == Movie.Data.TituloOriginal))
            {
                ListFechas(peli.DiasDisponiblesTodosCinemas);
            }
            InitView2();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            imgBackground.ImageSource = Utilities.ImageSelected;
            MovieName = Utilities.CapitalizeFirstLetter(Movie.Data.TituloOriginal);
            TxtTitle.Text = MovieName;

            var time = TimeSpan.FromMinutes(double.Parse(Movie.Data.Duracion));
            TxtDuracion.Text = string.Format("Duración: {0:00}h : {1:00}m", (int)time.TotalHours, time.Minutes);

            DateTime fechaActual = Utilities.FechaSeleccionada;

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
            if (Utilities.Movie.Data.Censura != "Todos")
            {
                frmModal modal = new frmModal(Utilities.GetConfiguration("MensajeCensura"));
                modal.ShowDialog();
            }
            ActivateTimer();
        }

        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        void ActivateTimer()
        {
            tbTimer.Text = Utilities.GetConfiguration("TimerHorario");
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
                    tbTimer.Text = "Tiempo de transacción: " + response;
                    tbHoraActual.Text = "Hora actual: " + DateTime.Now.ToString("HH:mm:ss");
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
        //Método para generar funciones para las películas
        public void GenerateFunctions()
        {
            try
            {
                int i = 0;
                lstGrid.Clear();
                //Se arma una lista con los datos de sólo el cinema donde se encuentra la máquina
                int Hour = DateTime.Now.Hour;

                foreach (var peli in Utilities.Peliculas.Pelicula.Where(pe => pe.Data.TituloOriginal == Movie.Data.TituloOriginal))
                {

                    //ListFechas(peli.DiasDisponiblesTodosCinemas);
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
                                var fechaSeleccionada = GetDateCorrectly(Utilities.FechaSeleccionada.ToString("yyyyMMdd"));
                                if (datetime == fechaSeleccionada)
                                {

                                    var schedules = function.Hora.OrderBy(h => h.Militar).ToList();
                                    List<HoraTMP> horatmps = new List<HoraTMP>();
                                    int count = 0;
                                    int countAvailable = 0;
                                    bool available = true;

                                    foreach (var item in schedules)
                                    {

                                        if (fechaSeleccionada != DateTime.Today)
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
                                        else if (int.Parse(item.Militar) >= int.Parse(DateTime.Now.AddMinutes(-40).ToString("HHmm")))
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
                                        lstGrid.Add(new Schedule
                                        {
                                            Title = Utilities.CapitalizeFirstLetter(Movie.Data.TituloOriginal),
                                            FontS = FontS,
                                            Language = string.Concat(peli.Data.Idioma),

                                            Gener = Movie.Data.Genero,
                                            Duration = string.Concat(Movie.Data.Duracion, " minutos"),
                                            Category = Movie.Data.Censura,
                                            Date = string.Concat(days[1], " ", days[2], " ", days[3]),
                                            Room = string.Concat("Sala ", room.NumeroSala),
                                            Hours = horatmps,
                                            TipoSala = ruta,
                                            RoomId = Convert.ToInt16(room.NumeroSala),
                                            UnivDate = function.Univ,
                                            MovieId = Convert.ToInt16(peli.Id),
                                            Formato = peli.Data.Formato,
                                            Censura = peli.Data.Censura
                                        });
                                        foreach (var newHoras in horatmps)
                                        {
                                            newHoras.DatosPelicula = new Schedule
                                            {
                                                Title = Utilities.CapitalizeFirstLetter(Movie.Data.TituloOriginal),
                                                FontS = FontS,
                                                Language = string.Concat(peli.Data.Idioma),
                                                Gener = Movie.Data.Genero,
                                                Duration = string.Concat(Movie.Data.Duracion, " minutos"),
                                                Category = Movie.Data.Censura,
                                                Date = string.Concat(days[1], " ", days[2], " ", days[3]),
                                                Room = string.Concat("Sala ", room.NumeroSala),
                                                Hour = newHoras.Horario,
                                                TipoSala = ruta,
                                                RoomId = Convert.ToInt16(room.NumeroSala),
                                                UnivDate = function.Univ,
                                                MovieId = Convert.ToInt16(peli.Id),
                                                Formato = peli.Data.Formato,
                                                Censura = peli.Data.Censura
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
                    Utilities.ShowModal("No hay funciones disponibles en esta fecha: " + Utilities.FechaSeleccionada.ToLongDateString());
                    return;
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "GenerateFunctions en frmSchudele", EError.Aplication, ELevelError.Medium);
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
                case "3D":
                    return "/Images/Tags/cartel-3d.png";
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
            lvSchedule.DataContext = lstGrid;
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
                UnivDate = selectedSchedule.DatosPelicula.UnivDate,
                Censura = selectedSchedule.DatosPelicula.Censura
            };
            //Schedule schedule = (Schedule)lvSchedule.SelectedItem;
            Utilities.MovieFormat = selectedSchedule.DatosPelicula.Formato;
            DipMap dipmap = SetProperties(schedule);

            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            //Utilities.ResetTimer();
            frmSeat frmSeat = new frmSeat(dipmap);
            frmSeat.Show();
            Close();
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
            frmMovies frmMovies = new frmMovies();
            frmMovies.Show();
            Close();
        }

        #endregion

        #region Dias Disponibles

        #region Metodos
        int controlFechaActual = 0;
        private void ListFechas(DiasDisponiblesTodosCinemas dias)
        {
            try
            {
                foreach (var item in dias.Dia)
                {
                    string fechaCompuesta = ConvertDate(item);
                    DateTime dt2 = DateTime.Parse(fechaCompuesta);
                    var datetime = GetDateCorrectly(item.Univ);

                    if (datetime >= DateTime.Today)
                    {
                        string NombreDiaAdd = dt2.ToString("dddd", CultureInfo.CreateSpecificCulture("es-ES"));
                        var exist = dateName.Where(dt => dt.Mes == dt2.ToString("MMM").ToUpper() && dt.DiaNumero == dt2.ToString("dd")).Count();
                        if (exist == 0)
                        {
                            dateName.Add(new DateName
                            {
                                FechaOrigin = dt2,
                                Mes = dt2.ToString("MMM").ToUpper(),
                                NombreDia = NombreDiaAdd.ToUpper(),
                                DiaNumero = dt2.ToString("dd")
                            });

                            dateName = dateName.OrderBy(i => i.FechaOrigin).ToList();
                        }
                    }

                }


                Utilities.FechaSeleccionada = dateName[0].FechaOrigin;


            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "ListFechas en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private static string ConvertDate(Dia item)
        {
            try
            {
                string anio = item.Univ.Substring(0, 4);
                string mes = item.Univ.Substring(4, 2);
                string dia = item.Univ.Substring(6, 2);
                string fechaCompuesta = string.Concat(anio, "/", mes, "/", dia);
                return fechaCompuesta;
            }
            catch (Exception ex)
            {
                return DateTime.Today.ToString();
            }
        }

        private void InitView2()
        {
            try
            {
                if (dateName.Count() > 0)
                {
                    foreach (var item in dateName)
                    {
                        lstPager2.Add(new DateName
                        {
                            Mes = item.Mes,
                            NombreDia = item.NombreDia,
                            FechaOrigin = item.FechaOrigin,
                            TextColor = "Black",
                            DiaNumero = item.DiaNumero
                        });
                    }

                    CreatePages2(dateName.Count());
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "InitView2 en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private void CreatePages2(int i)
        {
            try
            {
                int itemcount = i;
                totalPage2 = itemcount / itemPerPage2;
                if (itemcount % itemPerPage2 != 0)
                {
                    totalPage2 += 1;
                }
                lstPager2[0].TextColor = "#FF1385FF";
                view2.Source = lstPager2;
                view2.Filter += new FilterEventHandler(View_Filter2);
                lv_DateName.DataContext = view2;
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "CreatePages2 en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        #endregion

        #region Eventos
        private void View_Filter2(object sender, FilterEventArgs e)
        {
            try
            {
                int index = lstPager2.IndexOf((DateName)e.Item);

                if (index >= itemPerPage2 * currentPageIndex2 && index < itemPerPage2 * (currentPageIndex2 + 1))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "View_Filter2 en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private async void lv_DateName_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                var service = (DateName)(sender as ListViewItem).Content;

                FechaSelect = service.FechaOrigin;//poner la fecha en el formato xml
                Utilities.FechaSeleccionada = FechaSelect;
                GenerateFunctions();
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "lv_Datename_previewstylusDown en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private void btnNext2_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                SetCallBacksNull();
                ActivateTimer();
                if (currentPageIndex2 < totalPage2 - 1)
                {
                    currentPageIndex2++;
                    view2.View.Refresh();
                }
                if (currentPageIndex2 == totalPage2 - 1)
                {
                    btnNext2.Visibility = Visibility.Hidden;
                }

                btnPrev2.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "btnNext en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private void Grid_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                var childs = (sender as Grid).Children;
                foreach (var item in childs)
                {
                    if (item is Border)
                    {
                        ClearHoursList();
                        var border = item as Border;
                        Color color2 = (Color)ColorConverter.ConvertFromString("#FFF89126");
                        border.Background = new SolidColorBrush(color2);
                        borders.Add(border);
                    }
                }
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "Grid_PreviewStylus en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }

        private void ClearHoursList()
        {
            try
            {
                foreach (var item in borders)
                {
                    item.Background = Brushes.White;
                }
            }
            catch { }
        }

        private void btnPrev2_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                SetCallBacksNull();
                ActivateTimer();
                if (currentPageIndex2 > 0)
                {
                    currentPageIndex2--;
                    view2.View.Refresh();
                }

                if (currentPageIndex2 == 0)
                {
                    btnPrev2.Visibility = Visibility.Hidden;
                }

                btnNext2.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                AdminPaypad.SaveErrorControl(ex.Message, "btnPrev2 en frmSchudele", EError.Aplication, ELevelError.Medium);
            }
        }
        #endregion

        #endregion
    }
}
